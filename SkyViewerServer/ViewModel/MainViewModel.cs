using Fleck;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace SkyViewerServer.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private WebSocketServer webSocketServer;
        public MainViewModel()
        {
            ClientConnections = new ObservableCollection<ClientConnection>();

            this.StartCommand = new RelayCommand(this.StartCommand_Excuted);

            this.CloseCommand = new RelayCommand(this.CloseCommand_Excuted);
        }

        /// <summary>
        /// 启动浏览器
        /// </summary>
        public RelayCommand StartCommand { get; set; }

        /// <summary>
        /// 启动浏览器
        /// </summary>
        public RelayCommand CloseCommand { get; set; }

        private bool isServerStarted;
        /// <summary>
        /// Server是否已经启动
        /// </summary>
        public bool IsServerStarted
        {
            get { return isServerStarted; }
            set
            {
                isServerStarted = value;
                RaisePropertyChanged("IsServerStarted");
            }
        }

        private ObservableCollection<ClientConnection> clientConnections;

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<ClientConnection> ClientConnections
        {
            get { return clientConnections; }
            set
            {
                clientConnections = value;
                RaisePropertyChanged("ClientConnections");
            }
        }

        private void StartCommand_Excuted()
        {
            FleckLog.Level = LogLevel.Debug;

            if (IsServerStarted) return;
            try
            {
                webSocketServer = new WebSocketServer("ws://0.0.0.0:8181");

                webSocketServer.Start(socket =>
                {
                    socket.OnOpen = () =>
                    {
                        AddConnection(socket);
                    };
                    socket.OnClose = () =>
                    {
                        RemoveConnection(socket);
                    };
                    socket.OnMessage = message =>
                    {
                        ReceiveMessage(socket, message);
                    };
                    socket.OnBinary = message =>
                    {

                    };
                });
                IsServerStarted = true;
            }
            catch (Exception)
            {
                IsServerStarted = false;
            }
        }

        private void AddConnection(IWebSocketConnection socket)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    foreach (var item in ClientConnections)
                    {
                        if (item.ClientIpAddress.Equals(socket.ConnectionInfo.ClientIpAddress))
                        {
                            return;
                        }
                    }

                    ClientConnections.Add(new ClientConnection(socket));
                    Console.WriteLine("当前连接数" + ClientConnections.Count);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("当前连接数" + ClientConnections.Count);
                }
            }));
        }


        private void RemoveConnection(IWebSocketConnection socket)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                ClientConnection conn = null;
                foreach (var item in ClientConnections)
                {
                    if (item.ClientIpAddress.Equals(socket.ConnectionInfo.ClientIpAddress))
                    {
                        conn = item;
                    }
                }
                if (conn != null)
                {
                    ClientConnections.Remove(conn);
                }
            }));
        }

        private void ReceiveMessage(IWebSocketConnection socket, string message)
        {
            ClientConnection conn = null;
            foreach (var item in ClientConnections)
            {
                if (item.ClientIpAddress.Equals(socket.ConnectionInfo.ClientIpAddress))
                {
                    conn = item;
                }
            }
            if (conn != null)
            {
                conn.Message = message;
            }
        }

        private void CloseCommand_Excuted()
        {
            try
            {
                foreach (var conn in ClientConnections)
                {
                    conn.Close();
                }

                webSocketServer.Dispose();
            }
            catch (Exception)
            {

            }
            IsServerStarted = false;
        }
    }
}