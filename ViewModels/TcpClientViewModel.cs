using System.Windows;
using ZConnect.Models;
using ZConnect.Services;
using ZConnect.Utils;

namespace ZConnect.ViewModels
{
    /// <summary>
    /// Tcp client ViewModel.
    /// Used to bind logic to UI.
    /// </summary>
    public class TcpClientViewModel : BaseCommunicationViewModel
    {
        private readonly TcpClientService _service = new();

        public TcpConnectionModel Connection => _service.Connection;

        public TcpClientViewModel()     // Constructor function, a service parameter of type TcpClientService is required; (: base(service))：first, Call the constructor of the base class.
        {
            SendAction = _service.SendAsync;
            _service.StatusChanged += (s, e) =>      // s: sender, this is service; e: eventargs, Data, Connection, StatusType...
            {
                Application.Current.Dispatcher.BeginInvoke(() =>     // Dispatcher is the UI scheduler.
                {
                    OnStatusChanged(s, e as ICommunicationStatus);  // Event subscription (event_name += method_name). Whenever _service.StatusChanged is called, the OnStatusChanged method is automatically called.
                });
            };
        }

        public async Task ConnectAsync()
        {
            await _service.ConnectAsync(Connection.RemoteIp!, Connection.RemotePort);   // ((TcpClientService)_service): Convert the type of _service to TcpClientService type.
        }

        public void Disconnect()
        {
            _service.Disconnect();
            AutoSend = false;
        }
    }
}
