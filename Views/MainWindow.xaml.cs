using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZConnect.ViewModels;
using ZConnect.Views;

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

        
    }
}