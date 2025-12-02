using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ZConnect.Models;
using ZConnect.Models.Events;
using ZConnect.Services;
using ZConnect.Utils;

namespace ZConnect.ViewModels
{
    public class UdpClientViewModel : BaseCommunicationViewModel
    {
        private readonly UdpClientService _service = new();

        public UdpConnectionModel Connection => _service.Connection;

        public UdpClientViewModel()
        {
            SendAction = _service.SendAsync;
            _service.StatusChanged += (s, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    OnStatusChanged(s, e as ICommunicationStatus);
                });
            };
        }

        public void Connect()
        {
            if (string.IsNullOrEmpty(Connection.LocalIp) || string.IsNullOrEmpty(Connection.RemoteIp))
                throw new InvalidOperationException("Local/Remote IP must be set.");
            _service.Connect(Connection.LocalIp, Connection.LocalPort, Connection.RemoteIp, Connection.RemotePort);
        }

        public void Close()
        {
            _service.Close();
            AutoSend = false;
        }
    }
}
