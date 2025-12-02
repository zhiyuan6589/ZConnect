using System.Windows;
using ZConnect.Models;
using ZConnect.Services;
using ZConnect.Utils;

namespace ZConnect.ViewModels
{
    /// <summary>
    /// Tcp service ViewModel.
    /// Used to bind logic to UI.
    /// </summary>
    public class TcpServerViewModel : BaseCommunicationViewModel
    {
        private readonly TcpServerService _service = new();

        public TcpConnectionModel Connection => _service.Connection;

        public TcpServerViewModel()
        {
            SendAction = _service.SendAsync;
            _service.StatusChanged += (s, e) =>
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    OnStatusChanged(s, e as ICommunicationStatus);
                });
            };
        }

        public async Task StartAsync()
        {
            await _service.StartAsync(Connection.LocalIp!, Connection.LocalPort);
        }

        public void Stop()
        {
            _service.Stop();
            AutoSend = false;
        }
    }
}
