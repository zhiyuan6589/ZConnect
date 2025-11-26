using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using ZConnect.Models;
using ZConnect.Services;
using ZConnect.Utils;

namespace ZConnect.ViewModels
{
    public class TcpServerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private readonly TcpServerService _service = new TcpServerService();
        public TcpConnectionModel Connection => _service.Connection;

        private string _tcpStatus;
        public string TcpStatus
        {
            get => _tcpStatus;
            set { _tcpStatus = value; OnPropertyChanged(); }
        }

        private string _receivedText = "";  // Private property, access and update through the public property ReceivedText.
        public string ReceivedText
        {
            get => _receivedText;   // When getting Received property value, return the value of _receivedText property.
            set { _receivedText = value; OnPropertyChanged(); } // When setting the ReceivedText property value, modify the _receivedText property value and call the OnPropertyChanged method to nodify the UI that the property value has been updated.
        }

        private string _sendText = "";
        public string SendText
        {
            get => _sendText;
            set { _sendText = value; OnPropertyChanged(); }
        }

        private string? _statusMessage = "";
        public string? StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        public DataFormatEnum SendFormat { get; set; } = DataFormatEnum.String;

        public DataFormatEnum ReceiveFormat { get; set; }

        private bool _autoSend = false;
        public bool AutoSend
        {
            get => _autoSend;
            set { _autoSend = value; OnPropertyChanged(); }
        }

        private bool _isAutoSending = false;

        private int _intervalMs = 1000;
        public int IntervalMs
        {
            get => _intervalMs;
            set { _intervalMs = value; OnPropertyChanged(); }
        }

        public TcpServerViewModel()
        {
            _service.StatusChanged += OnStatusChanged;
        }

        private void OnStatusChanged(object? sender, TcpStatusChangedEventArgs args)
        {
            if (args.StatusType == TcpStatusEnum.DataReceived && args.Data != null)
            {
                string text = ConvertReceived(args.Data, ReceiveFormat);
                ReceivedText += $"[Recv {DateTime.Now:HH:mm:ss}] {text}\n";
            }

            switch (args.StatusType)
            {
                case TcpStatusEnum.Listening:
                    TcpStatus = "Listening";
                    break;
                case TcpStatusEnum.NotListening:
                    TcpStatus = "NotListening";
                    break;
                case TcpStatusEnum.Connected:
                    TcpStatus = "Connected";
                    break;
                case TcpStatusEnum.Disconnected:
                    TcpStatus = "Disconnected";
                    break;
                case TcpStatusEnum.DataReceived:
                    TcpStatus = "DataReceived";
                    break;
                case TcpStatusEnum.DataSent:
                    TcpStatus = "DataSent";
                    break;
                case TcpStatusEnum.Error:
                    TcpStatus = "Error";
                    break;
                default:
                    break;
            }

            StatusMessage = args.Message;
        }

        public async Task StartAsync()
        {
            await _service.StartAsync(Connection.LocalIp!, Connection.LocalPort);
        }

        public async Task SendAsync()
        {
            if (_isAutoSending) return;
            if (string.IsNullOrEmpty(SendText)) return;
            byte[] data = ConvertSend(SendText, SendFormat);

            _isAutoSending = AutoSend;
            do
            {
                await _service.SendAsync(data);
                ReceivedText += $"[Send {DateTime.Now:HH:mm:ss}] {ConvertReceived(data, SendFormat)}\n";    // The recived/send text record.
                if (!AutoSend || !Connection.IsConnected) break;
                await Task.Delay(IntervalMs);
            } while (AutoSend && (Connection.IsConnected));
            _isAutoSending = false;
        }

        public void Stop()
        {
            _service.Stop();
        }

        private byte[] ConvertSend(string input, DataFormatEnum format)
        {
            return format switch
            {
                DataFormatEnum.String => Encoding.UTF8.GetBytes(input),
                DataFormatEnum.Hex => FormatConverter.HexToBytes(input),
                DataFormatEnum.Ascii => Encoding.ASCII.GetBytes(input),
                _ => throw new NotSupportedException(),
            };
        }

        private string ConvertReceived(byte[] data, DataFormatEnum format)
        {
            return format switch
            {
                DataFormatEnum.String => Encoding.UTF8.GetString(data),
                DataFormatEnum.Hex => FormatConverter.BytesToHex(data),
                DataFormatEnum.Ascii => FormatConverter.BytesToAscii(data),
                _ => throw new NotSupportedException(),
            };
        }
    }
}
