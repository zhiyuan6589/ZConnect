using CommunityToolkit.Mvvm.Input;
using System.Windows;
using ZConnect.Models;
using ZConnect.Services;

namespace ZConnect.ViewModels
{
    public partial class UdpClientViewModel : BaseCommunicationViewModel
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

        [RelayCommand]
        private void Connect()
        {
            if (string.IsNullOrEmpty(Connection.LocalIp) || string.IsNullOrEmpty(Connection.RemoteIp))
                throw new InvalidOperationException("Local/Remote IP must be set.");
            _service.Connect(Connection.LocalIp, Connection.LocalPort, Connection.RemoteIp, Connection.RemotePort);
        }

        [RelayCommand]
        private void Close()
        {
            _service.Close();
            AutoSend = false;
        }
    }
}
