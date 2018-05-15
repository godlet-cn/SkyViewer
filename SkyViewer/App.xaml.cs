using System.Windows;

namespace SkyViewer
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            SkyViewer.MainWindow maiWindow = new MainWindow();

            maiWindow.Show();
        }
    }
}
