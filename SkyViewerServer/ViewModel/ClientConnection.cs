using Common;
using Fleck;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Threading.Tasks;

namespace SkyViewerServer
{
    public class ClientConnection : ViewModelBase
    {
        private IWebSocketConnection conn;

        private ClientSetting ClientSeeting;

        public ClientConnection(IWebSocketConnection conn)
        {
            if (conn == null) throw new ArgumentNullException();

            this.conn = conn;

            this.ClientSeeting = new ClientSetting();

            this.ClientIpAddress = this.conn.ConnectionInfo.ClientIpAddress;

            this.StartCommand = new RelayCommand(this.StartCommand_Excuted);

            this.CloseCommand = new RelayCommand(this.CloseCommand_Excuted);

            this.SettingCommand = new RelayCommand(this.SettingCommand_Excuted);
        }
        
        private string clientIpAddress;

        /// <summary>
        /// 是否显示画笔属性面板
        /// </summary>
        public string ClientIpAddress
        {
            get { return clientIpAddress; }
            set
            {
                clientIpAddress = value;
                RaisePropertyChanged("ClientIpAddress");
            }
        }

        private string message;

        /// <summary>
        /// 收到的消息
        /// </summary>
        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                RaisePropertyChanged("Message");
            }
        }

        /// <summary>
        /// 启动浏览器
        /// </summary>
        public RelayCommand StartCommand { get; set; }


        /// <summary>
        /// 启动浏览器
        /// </summary>
        public RelayCommand CloseCommand { get; set; }


        /// <summary>
        /// 启动浏览器
        /// </summary>
        public RelayCommand SettingCommand { get; set; }


        private void StartCommand_Excuted()
        {
            this.conn.Send("START");
        }

        private void CloseCommand_Excuted()
        {
            this.conn.Send("CLOSE");
        }

        private void SettingCommand_Excuted()
        {
            WinSetting winSet = new WinSetting(this.ClientSeeting);

            winSet.SettingChanged += WinSet_SettingChanged;
            winSet.ShowDialog();
        }

        private void WinSet_SettingChanged(ClientSetting setting)
        {
            //if (this.ClientSeeting.HomeUrl.Equals(setting.HomeUrl)==false)
            //{
            this.conn.Send("HOME " + setting.HomeUrl);
            //}

            //if (ClientSeeting.Location.X != setting.Location.X
            //    || ClientSeeting.Location.Y != setting.Location.Y
            //    || ClientSeeting.Location.Width != setting.Location.Width
            //    || ClientSeeting.Location.Height != setting.Location.Height)
            //{
            this.conn.Send("LOCATION " + string.Format("{0},{1},{2},{3}",
                setting.Location.X, setting.Location.Y, setting.Location.Width, setting.Location.Height));
            //}
            this.ClientSeeting = setting;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task Send(string message)
        {
            return this.conn.Send(message);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task Send(byte[] message)
        {
            return this.conn.Send(message);
        }

        /// <summary>
        /// 关闭客户端
        /// </summary>
        public void Close()
        {
            this.conn.Close();
        }
    }
}
