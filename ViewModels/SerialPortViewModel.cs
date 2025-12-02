using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using ZConnect.Models;
using ZConnect.Services;
using ZConnect.Utils;

namespace ZConnect.ViewModels
{
    public class SerialPortViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private readonly SerialPortService _service = new();

        public SerialPortConnectionModel Connection => _service.Connection;

        private string _serialPortStatus = "Closed";
        public string SerialPortStatus
        {
            get => _serialPortStatus;
            set { _serialPortStatus = value; OnPropertyChanged(); }
        }

        private string _receivedText = "";
        public string ReceivedText
        {
            get => _receivedText;
            set { _receivedText = value; OnPropertyChanged(); }
        }

        private string _sendText = "";
        public string SendText
        {
            get => _sendText;
            set { _sendText = value; OnPropertyChanged(); }
        }

        public DataFormatEnum SendFormat { get; set; } = DataFormatEnum.String;
        public DataFormatEnum ReceiveFormat { get; set; }

        private bool _autoSend = false;
        public bool AutoSend
        {
            get => _autoSend;
            set { _autoSend = value; OnPropertyChanged(); }
        }

        protected bool _isAutoSending = false;

        private int _intervalMs = 1000;
        public int IntervalMs
        {
            get => _intervalMs;
            set { _intervalMs = value; OnPropertyChanged(); }
        }

        public ObservableCollection<int> StandardBaudRates { get; } = new ObservableCollection<int>     // todo
        {
            110, 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 56000, 57600, 115200, 230400, 256000, 460800, 500000, 512000, 600000, 750000, 921600
        };

        public ObservableCollection<Parity> ParityValues { get; } =     // todo
            new ObservableCollection<Parity>((Parity[])Enum.GetValues(typeof(Parity)));

        public ObservableCollection<int> DataBitsValues { get; } = new ObservableCollection<int>
        {
            5, 6, 7, 8
        };

        public Array StopBitsValues { get; } = Enum.GetValues(typeof(StopBits));

        public Array HandshakeValues { get; } = Enum.GetValues(typeof(Handshake));
        
        public SerialPortViewModel()
        {
            _service.StatusChanged += (s, e) =>
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    OnStatusChanged(s, e);
                });
            };
        }

        protected void OnStatusChanged(object? sender, SerialPortStatusChangedEventArgs args)
        {
            if (args.StatusType == SerialPortStatusEnum.DataReceived && args.Data != null)
            {
                string text = FormatConverter.ConvertReceived(args.Data, ReceiveFormat);
                ReceivedText += $"[Recv {DateTime.Now:HH:mm:ss}] {text}\n";
            }

            SerialPortStatus = args.StatusType.ToString();
        }

        public void Open()
        {
            if ((!string.IsNullOrEmpty(Connection.PortName)) && (SerialPortStatus != "Opened"))
                _service.Open(Connection.PortName, Connection.BaudRate, Connection.Parity, Connection.DataBits, Connection.StopBits, Connection.Handshake);
        }

        public async Task WriteAsync()
        {
            if (_isAutoSending) return;
            if (string.IsNullOrEmpty(SendText)) return;

            byte[] data = FormatConverter.ConvertSend(SendText, SendFormat);

            _isAutoSending = AutoSend;
            do
            {
                await _service.WriteAsync(data);
                ReceivedText += $"[Send {DateTime.Now:HH:mm:ss}] {FormatConverter.ConvertReceived(data, SendFormat)}\n";    // The recived/send text record.
                if (!AutoSend) break;
                await Task.Delay(IntervalMs).ConfigureAwait(false);     // There is no switch back to the UI thread. Each loop only returns to the UI thread after writing data and updating the UI.
            } while (AutoSend);
            _isAutoSending = false;
        }

        public void Close()
        {
            _service.Close();
            ReceivedText = "";
        }

        public void ClearText()
        {
            ReceivedText = "";
        }
    }
}
