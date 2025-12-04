using System.Windows;
using System.Windows.Controls;
using ZConnect.Services;
using ZConnect.ViewModels;

namespace ZConnect.Views
{
    /// <summary>
    /// TcpClientView.xaml 的交互逻辑
    /// </summary>
    public partial class TcpClientView : UserControl
    {
        private readonly TcpClientViewModel _vm = new();
        public TcpClientView()
        {
            InitializeComponent();  // Load the UI defined by the XMAL file, initialize the control object, set control object properties, event binding, etc.
            DataContext = _vm;  // Context object for data binding in WPF
        }
    }
}
