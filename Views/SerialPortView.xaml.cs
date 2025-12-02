using System.Windows;
using System.Windows.Controls;
using ZConnect.ViewModels;

namespace ZConnect.Views
{
    /// <summary>
    /// SerialPortView.xaml 的交互逻辑
    /// </summary>
    public partial class SerialPortView : UserControl
    {
        private readonly SerialPortViewModel _vm = new();
        public SerialPortView()
        {
            InitializeComponent();
            DataContext = _vm;
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            _vm.Open();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            _vm.Close();
        }

        private async void Write_Click(object sender, RoutedEventArgs e)
        {
            await _vm.WriteAsync();
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
