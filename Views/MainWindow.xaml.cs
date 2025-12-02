using System.Windows;

namespace ZConnect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainContent.Content = FindResource("TcpServerView");
        }

        private void ShowTcpClient(object sender, RoutedEventArgs e)
        {
            MainContent.Content = FindResource("TcpClientView");
        }

        private void ShowTcpServer(object sender, RoutedEventArgs e)
        {
            MainContent.Content = FindResource("TcpServerView");
        }

        private void ShowUdpClient(object sender, RoutedEventArgs e)
        {
            MainContent.Content = FindResource("UdpClientView");
        }

        private void ShowSerialPort(object sender, RoutedEventArgs e)
        {
            MainContent.Content = FindResource("SerialPortView");
        }
    }
}