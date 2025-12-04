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
    }
}
