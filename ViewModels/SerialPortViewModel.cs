using CommunityToolkit.Mvvm.Input;
using System.IO.Ports;
using System.Windows;
using ZConnect.Models;
using ZConnect.Services;

namespace ZConnect.ViewModels
{
    public partial class SerialPortViewModel : BaseCommunicationViewModel
    {
        private readonly SerialPortService _service = new();

        public SerialPortConnectionModel Connection => _service.Connection;

        public int[] StandardBaudRates { get; } = [110, 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 56000, 57600, 115200, 230400, 256000, 460800, 500000, 512000, 600000, 750000, 921600];

        public Array ParityValues { get; } = Enum.GetValues<Parity>();

        public int[] DataBitsValues { get; } = [5, 6, 7, 8];

        public Array StopBitsValues { get; } = Enum.GetValues<StopBits>();

        public Array HandshakeValues { get; } = Enum.GetValues<Handshake>();

        public SerialPortViewModel()
        {
            SendAction = _service.SendAsync;    // "Inject" the sending method of the communication service into the ViewModel as a callable function variable to achieve module decoupling
            _service.StatusChanged += (s, e) =>
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    OnStatusChanged(s, e as ICommunicationStatus);  // Try to convert the object to the ICommunicationStatus interface type. If it fails, it will return null and no exception will be thrown.
                });
            };
        }

        [RelayCommand]
        private void Open()
        {
            if ((!string.IsNullOrEmpty(Connection.PortName)) && (Status != "Opened"))
                _service.Open(Connection.PortName, Connection.BaudRate, Connection.Parity, Connection.DataBits, Connection.StopBits, Connection.Handshake);
        }

        [RelayCommand]
        private void Close()
        {
            _service.Close();
            AutoSend = false;
        }
    }
}
