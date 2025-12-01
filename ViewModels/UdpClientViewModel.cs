using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.RightsManagement;
using System.Text;
using ZConnect.Models;
using ZConnect.Models.Events;
using ZConnect.Services;
using ZConnect.Utils;

namespace ZConnect.ViewModels
{
    public class UdpClientViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private readonly UdpClientService _service = new();

        public UdpConnectionModel Connection => _service.Connection;

        private string _udpStatus = "Stopped";
        public string UdpStatus
        {
            get => _udpStatus;
            set { _udpStatus = value; OnPropertyChanged(); }
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

        public UdpClientViewModel()
        {
            _service.StatusChanged += OnStatusChanged;
        }

        protected void OnStatusChanged(object? sender, UdpStatusChangedEventArgs args)
        {
            if (args.StatusType == UdpStatusEnum.DataReceived && args.Data != null)
            {
                string text = FormatConverter.ConvertReceived(args.Data, ReceiveFormat);
                ReceivedText += $"[Recv {DateTime.Now:HH:mm:ss}] {text}\n";
            }

            UdpStatus = args.StatusType.ToString();
        }

        public void Connect()
        {
            if (string.IsNullOrEmpty(Connection.LocalIp) || string.IsNullOrEmpty(Connection.RemoteIp))
                throw new InvalidOperationException("Local/Remote IP must be set.");
            _service.Connect(Connection.LocalIp, Connection.LocalPort, Connection.RemoteIp, Connection.RemotePort);
        }

        public async Task SendAsync()
        {
            if (_isAutoSending) return;
            if (string.IsNullOrEmpty(SendText)) return;

            byte[] data = FormatConverter.ConvertSend(SendText, SendFormat);

            _isAutoSending = AutoSend;
            do
            {
                await _service.SendAsync(data);
                ReceivedText += $"[Send {DateTime.Now:HH:mm:ss}] {FormatConverter.ConvertReceived(data, SendFormat)}\n";    // The recived/send text record.
                if (!AutoSend) break;
                await Task.Delay(IntervalMs);
            } while (AutoSend);
            _isAutoSending = false;
        }

        public void Close()
        {
            _service.Close();
        }

        public void ClearText()
        {
            ReceivedText = "";
        }
    }
}
