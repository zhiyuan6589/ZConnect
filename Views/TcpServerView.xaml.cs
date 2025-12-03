using System.Windows;
using System.Windows.Controls;
using ZConnect.Services;
using ZConnect.ViewModels;

namespace ZConnect.Views
{
    /// <summary>
    /// TcpServerView.xaml 的交互逻辑
    /// </summary>
    public partial class TcpServerView : UserControl
    {
        private readonly TcpServerViewModel _vm = new();
        public TcpServerView()
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
