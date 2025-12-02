using System.Windows;
using System.Windows.Controls;
using ZConnect.ViewModels;

namespace ZConnect.Views
{
    /// <summary>
    /// UdpClientView.xaml 的交互逻辑
    /// </summary>
    public partial class UdpClientView : UserControl
    {
        private readonly UdpClientViewModel _vm = new();
        public UdpClientView()
        {
            InitializeComponent();
            DataContext = _vm;
        }

        private void Start_Click(object sender, RoutedEventArgs e)  // object sender: The control object that triggered the event (button here); RoutedEventArgs: Event parameters, including event-related information (such as routing, source, etc.)
        {
            _vm.Connect();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            _vm.Close();
        }

        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            await _vm.SendAsync();
        }

        private void ClearText_Click(object sender, RoutedEventArgs e)
        {
            _vm.ClearText();
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var sv = sender as ScrollViewer;

            if (e.ExtentHeightChange > 0)
            {
                sv?.ScrollToEnd();
            }
        }
    }
}
