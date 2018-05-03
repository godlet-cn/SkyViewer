using System.Windows;

namespace SkyViewerServer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        ServerViewModel viewModel;

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel = new ServerViewModel();
            this.DataContext = viewModel;
        }
    }
}
