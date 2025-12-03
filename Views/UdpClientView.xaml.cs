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
