using Common;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.ServiceProcess;
using System.Threading;
using WebSocketSharp;

namespace SkyViewerService
{
    public partial class SkyViewerService : ServiceBase
    {
        public SkyViewerService()
        {
            InitializeComponent();
        }

        private string appCachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "SkyViewer");
        private IniFileHelper iniSetting;
        
        private string wkSocketSercerUrl;
        private WebSocket webSocket;

        private string exeName = "SkyViewer.exe";
        private string channelUri;

        protected override void OnStart(string[] args)
        {
            try
            {
                iniSetting = new IniFileHelper(Path.Combine(appCachePath, "setting.ini"));

                //读取服务和应用程序通讯管道设置
                string channelName = iniSetting.IniReadValue("General", "ChannelName");
                if (string.IsNullOrEmpty(channelName))
                {
                    channelName = "SkyViewerChannel";
                    iniSetting.IniWriteValue("General", "ChannelName", channelName);
                }

                string strChannelPort = iniSetting.IniReadValue("General", "ChannelPort");
                int channelPort;
                bool isvalidPort = int.TryParse(strChannelPort, out channelPort);
                if (isvalidPort == false || channelPort == 0)
                {
                    channelPort = 6530;
                    iniSetting.IniWriteValue("General", "ChannelPort", channelPort.ToString());
                }
                channelUri = string.Format("tcp://localhost:{0}/ShareObject", channelPort);

                TcpServerChannel channel = new TcpServerChannel(channelName, channelPort);
                ChannelServices.RegisterChannel(channel, false);
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(ShareObject), "ShareObject", WellKnownObjectMode.Singleton);

                exeName = iniSetting.IniReadValue("General", "AppName");
                if (string.IsNullOrEmpty(exeName))
                {
                    exeName = "SkyViewer.exe";
                }

                wkSocketSercerUrl = iniSetting.IniReadValue("Service", "WebSocket");
                if (string.IsNullOrEmpty(wkSocketSercerUrl))
                {
                    LogManager.WiteLog("未设置服务器WebSocket地址");
                }

                ConnectWebSocketServer();

                LogManager.WiteLog("服务启动成功！");
            }
            catch (Exception ex)
            {
                LogManager.WiteLog("服务启动发送错误" + ex.Message + " " + ex.StackTrace);
            }
        }

        protected override void OnStop()
        {
            if (webSocket != null)
            {
                webSocket.Close();
            }
            LogManager.WiteLog("服务停止！");
        }

        private void ConnectWebSocketServer()
        {
            while (webSocket == null||(webSocket!=null&& webSocket.IsAlive==false))
            {
                webSocket = new WebSocket(wkSocketSercerUrl);
                webSocket.Connect();

                if (webSocket.IsAlive)
                {
                    webSocket.Send("CONNECTED");
                    StartService(webSocket);
                    
                    LogManager.WiteLog("连接WebSocket服务器成功");
                }

                Thread.Sleep(5000);
            }
        }

        private void StartService(WebSocket ws)
        {
            ws.OnMessage += (sender, e) =>
            {
                ShareObject objRemoteObject = (ShareObject)Activator.GetObject(typeof(ShareObject), channelUri);
                if (objRemoteObject == null) return;
                LogManager.WiteLog("接收到消息" + e.Data);

                string[] infos = e.Data.Split(' ');
                string command = infos[0].ToUpper();
                switch (command)
                {
                    case "START":
                        objRemoteObject.State = WinState.NORMAL;
                        if (IsSkyViewerStarted() == false)
                        {
                            StartSkyViewer();
                        }
                        ws.Send("STARTING");
                        break;
                    case "CLOSE":
                        objRemoteObject.State = WinState.CLOSE;
                        ws.Send("CLOSING");
                        break;
                    case "HOME":
                        if (infos.Length > 1)
                        {
                            string homeUrl = infos[1];
                            if (!string.IsNullOrEmpty(homeUrl))
                            {
                                objRemoteObject.HomeUrl = homeUrl;
                            }
                        }
                        ws.Send("HOME_SETTING");
                        break;
                    case "LOCATION":
                        if (infos.Length > 1)
                        {
                            string position = infos[1];
                            if (!string.IsNullOrEmpty(position))
                            {
                                string[] posInfos = position.Split(',');
                                objRemoteObject.X = double.Parse(posInfos[0]);
                                objRemoteObject.Y = double.Parse(posInfos[1]);
                                objRemoteObject.Width = double.Parse(posInfos[2]);
                                objRemoteObject.Height = double.Parse(posInfos[3]);
                            }
                        }
                        ws.Send("LOCATION_SETTING");
                        break;
                    default:
                        break;
                }
            };

            ws.OnError += (sender, e) =>
            {
                LogManager.WiteLog("检测到错误：" + e.Message);
            };

            ws.OnClose += (sender, e) =>
            {
                LogManager.WiteLog("检测到连接断开：" + e.Reason);
                ConnectWebSocketServer();
            };
        }

        /// <summary>
        /// 查找浏览器应用程序进程
        /// </summary>
        /// <returns></returns>
        private bool IsSkyViewerStarted()
        {
            Process[] th = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(exeName));
            if (th != null && th.Length > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 启动浏览器应用程序进程
        /// </summary>
        internal void StartSkyViewer()
        {
            try
            {
                string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, exeName);
                if (!File.Exists(exePath))
                {
                    LogManager.WiteLog("未找到应用程序！");
                    return;
                }
                WinAPI_Interop.CreateProcess(exePath);
            }
            catch (Exception ex)
            {
                LogManager.WiteLog("启动应用程序失败 " + ex.Message + " " + ex.StackTrace);
            }
        }

    }
}
