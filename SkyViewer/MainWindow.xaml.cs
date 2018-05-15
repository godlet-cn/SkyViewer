using CefSharp;
using Common;
using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Interop;

namespace SkyViewer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
        }

        private ClientSetting setting;
        private Thread receiveMessageThread;
        private IniFileHelper iniSetting;
        private string appCachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "SkyViewer");

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LogManager.WiteLog("启动浏览器窗口");

            browser.LifeSpanHandler = new OpenPageSelf();

            WindowHelper.SetForegroundWindowX(new WindowInteropHelper(this).Handle);

            setting = new ClientSetting();
            setting.HomeUrl = this.browser.Address;
            setting.Location = new ClientLocation()
            {
                X = this.Left,
                Y = this.Top,
                Width = this.Width,
                Height = this.Height
            };

            iniSetting = new IniFileHelper(Path.Combine(appCachePath, "setting.ini"));
            string channelPort = iniSetting.IniReadValue("General", "ChannelPort");
            string shareChannelUri = string.Format("tcp://localhost:{0}/ShareObject", channelPort);

            receiveMessageThread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    ShareObject remoteObject = (ShareObject)Activator.GetObject(typeof(ShareObject), shareChannelUri);
                    if (remoteObject != null)
                    {
                        HandleMessage(remoteObject);
                    }

                    Thread.Sleep(2000);
                }
            }));
            receiveMessageThread.Start();
        }

        private void HandleMessage(ShareObject remoteObject)
        {
            try
            {
                if (remoteObject.State == WinState.CLOSE)
                {
                    LogManager.WiteLog("关闭浏览器窗口");
                    remoteObject.State = WinState.NORMAL;
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (receiveMessageThread != null)
                        {
                            receiveMessageThread.Abort();
                        }
                        this.Close();
                    }));
                }

                if (!this.setting.HomeUrl.Equals(remoteObject.HomeUrl)
                && !string.IsNullOrEmpty(remoteObject.HomeUrl))
                {
                    LogManager.WiteLog("跳转到网页：" + remoteObject.HomeUrl);

                    this.setting.HomeUrl = remoteObject.HomeUrl;

                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.browser.Address = this.setting.HomeUrl;
                    }));
                }

                if (remoteObject.Width == 0 || remoteObject.Height == 0) return;

                if (this.setting.Location.Width != remoteObject.Width
                || this.setting.Location.Height != remoteObject.Height
                || this.setting.Location.X != remoteObject.X
                || this.setting.Location.Y != remoteObject.Y)
                {
                    ClientLocation location = new ClientLocation()
                    {
                        X = remoteObject.X,
                        Y = remoteObject.Y,
                        Width = remoteObject.Width,
                        Height = remoteObject.Height
                    };
                    this.setting.Location = location;

                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.Width = location.Width;
                        this.Height = location.Height;

                        LogManager.WiteLog("设置窗口位置：" + location.X + " " + location.Y + " " + location.Width + " " + location.Height);

                        IntPtr winptr = new WindowInteropHelper(this).Handle;
                        WindowHelper.SetWindowPos(winptr, WindowHelper.HWND_TOP, (int)location.X, (int)location.Y, 0, 0, WindowHelper.SWP_NOSIZE);
                        WindowHelper.SetForegroundWindowX(winptr);
                    }));
                }
            }
            catch (Exception ex)
            {
                LogManager.WiteLog(ex.Message + ":" + ex.StackTrace);
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (receiveMessageThread != null)
            {
                receiveMessageThread.Abort();
            }

            Cef.Shutdown();
        }

    }
}
