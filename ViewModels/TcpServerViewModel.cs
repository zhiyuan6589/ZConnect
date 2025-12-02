using System.Windows;
using ZConnect.Services;

namespace ZConnect.ViewModels
{
    /// <summary>
    /// Tcp service ViewModel.
    /// Used to bind logic to UI.
    /// </summary>
    public class TcpServerViewModel : BaseTcpViewMode
    {
        public TcpServerViewModel(TcpServerService service) : base(service)
        {
            service.StatusChanged += (s, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    OnStatusChanged(s, e);
                });
            };
        }

        public async Task StartAsync()
        {
            await ((TcpServerService)_service).StartAsync(Connection.LocalIp!, Connection.LocalPort);
        }

        public void Stop()
        {
            ((TcpServerService)_service).Stop();
        }
    }
}
