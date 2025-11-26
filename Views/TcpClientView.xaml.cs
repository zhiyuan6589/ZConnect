using System;
using System.Collections.Generic;
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

namespace ZConnect.Views
{
    /// <summary>
    /// TcpClientView.xaml 的交互逻辑
    /// </summary>
    public partial class TcpClientView : UserControl
    {
        private readonly TcpClientViewModel _vm = new TcpClientViewModel();
        public TcpClientView()
        {
            InitializeComponent();  // Load the UI defined by the XMAL file, initialize the control object, set control object properties, event binding, etc.
            DataContext = _vm;  // Context object for data binding in WPF
        }

        private async void Connect_Click(object sender, RoutedEventArgs e)  // object sender: The control object that triggered the event (button here); RoutedEventArgs: Event parameters, including event-related information (such as routing, source, etc.)
        {
            await _vm.ConnectAsync();
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            _vm.Disconnect();
        }

        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            await _vm.SendAsync();
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
