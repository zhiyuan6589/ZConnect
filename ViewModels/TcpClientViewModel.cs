using System.Windows;
using ZConnect.Services;

namespace ZConnect.ViewModels
{
    /// <summary>
    /// Tcp client ViewModel.
    /// Used to bind logic to UI.
    /// </summary>
    public class TcpClientViewModel : BaseTcpViewMode
    {
        public TcpClientViewModel(TcpClientService service) : base(service)     // Constructor function, a service parameter of type TcpClientService is required; (: base(service))：first, Call the constructor of the base class.
        {
            service.StatusChanged += (s, e) =>      // s: sender, this is service; e: eventargs, Data, Connection, StatusType...
            {
                Application.Current.Dispatcher.Invoke(() =>     // Dispatcher is the UI scheduler.
                {
                    OnStatusChanged(s, e);  // Event subscription (event_name += method_name). Whenever _service.StatusChanged is called, the OnStatusChanged method is automatically called.
                });
            };
        }

        public async Task ConnectAsync()
        {
            await ((TcpClientService)_service).ConnectAsync(Connection.RemoteIp!, Connection.RemotePort);   // ((TcpClientService)_service): Convert the type of _service to TcpClientService type.
        }

        public void Disconnect()
        {
            ((TcpClientService)_service).Disconnect();
        }
    }
}
