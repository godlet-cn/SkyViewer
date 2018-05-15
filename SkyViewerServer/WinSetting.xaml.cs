using Common;
using System.Windows;

namespace SkyViewerServer
{
    /// <summary>
    /// WinSetting.xaml 的交互逻辑
    /// </summary>
    public partial class WinSetting 
    {

        public WinSetting(ClientSetting setting)
        {
            InitializeComponent();

            this.tbHomeUrl.Text = setting.HomeUrl;
            this.tbPositionX.Text = setting.Location.X.ToString();
            this.tbPositionY.Text = setting.Location.Y.ToString();
            this.tbWidth.Text = setting.Location.Width.ToString();
            this.tbHeight.Text = setting.Location.Height.ToString();
        }
        
        public delegate void ClientSettingChanged(ClientSetting setting);
        public event ClientSettingChanged SettingChanged;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ClientSetting setting = new ClientSetting();
            setting.HomeUrl = this.tbHomeUrl.Text;

            ClientLocation location = new ClientLocation();
            double posX = 0;
            double.TryParse(this.tbPositionX.Text, out posX);
            location.X = posX;

            double posY = 0;
            double.TryParse(this.tbPositionY.Text, out posY);
            location.Y = posY;

            double width = 0;
            double.TryParse(this.tbWidth.Text, out width);
            location.Width = width;

            double height = 0;
            double.TryParse(this.tbHeight.Text, out height);
            location.Height = height;
            setting.Location = location;

            if (SettingChanged != null)
            {
                SettingChanged(setting);
            }
            this.Close();
        }
    }
}
