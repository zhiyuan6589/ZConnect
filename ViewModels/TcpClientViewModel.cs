using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.Runtime.CompilerServices;
using System.Text;
using ZConnect.Models;
using ZConnect.Services;
using ZConnect.Utils;

namespace ZConnect.ViewModels
{
    /// <summary>
    /// Tcp client ViewModel.
    /// Used to bind logic to UI.
    /// </summary>
    public class TcpClientViewModel : INotifyPropertyChanged    // Interface that notifies the UI that "a certain property has changed."
    {
        // MVVM Mode
        public event PropertyChangedEventHandler? PropertyChanged;  // Events are emitted by ViewModel, the UI automatically subscribes to the event through data binding.
        private void OnPropertyChanged([CallerMemberName] string? name = null)  // [CallerMemberName] is a syntactic sugar for C#, it will automatically pass in the property name that calls the method.
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private readonly TcpClientService _service = new TcpClientService();
        public TcpConnectionModel Connection => _service.Connection;    // Read-only property, equivalent to: get { return _service.Connect; }, the function to expose `read-only` access to _service.Connection.

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set { _isConnected = value; OnPropertyChanged(); }
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

        public TcpClientViewModel()
        {
            _service.StatusChanged += OnStatusChanged;  // Event subscription (event_name += method_name). Whenever _service.StatusChanged is called, the OnStatusChanged method is automatically called.
        }

        private void OnStatusChanged(object? sender, TcpStatusChangedEventArgs args)    // sender is the object itself that triggered this time.
        {
            if (args.StatusType == TcpStatusEnum.DataReceived && args.Data != null)
            {
                string text = ConvertReceived(args.Data, ReceiveFormat);
                ReceivedText += $"[Recv {DateTime.Now:HH:mm:ss}] {text}\n";
            }

            if (args.StatusType == TcpStatusEnum.Connected)
                IsConnected = true;
            else if (args.StatusType == TcpStatusEnum.Disconnected || args.StatusType == TcpStatusEnum.Error)
                IsConnected = false;

            StatusMessage = args.Message;
        }

        public async Task ConnectAsync()
        {
            await _service.ConnectAsync(Connection.RemoteIp!, Connection.RemotePort);
        }

        public async Task SendAsync()
        {
            if (string.IsNullOrEmpty(SendText)) return;

            byte[] data = ConvertSend(SendText, SendFormat);
            await _service.SendAsync(data);

            ReceivedText += $"[Send {DateTime.Now:HH:mm:ss}] {ConvertReceived(data, SendFormat)}\n"; // The recived/send text record.
        }

        public void Disconnect()
        {
            _service.Disconnect();
        }

        private byte[] ConvertSend(string input, DataFormatEnum format)
        {
            switch (format)
            {
                case DataFormatEnum.String:
                    return Encoding.UTF8.GetBytes(input);

                case DataFormatEnum.Hex:
                    return FormatConverter.HexToBytes(input);

                case DataFormatEnum.Ascii:
                    return Encoding.ASCII.GetBytes(input);

                default:
                    throw new NotSupportedException();
            }
        }

        private string ConvertReceived(byte[] data, DataFormatEnum format)
        {
            switch (format)
            {
                case DataFormatEnum.String:
                    return Encoding.UTF8.GetString(data);

                case DataFormatEnum.Hex:
                    return FormatConverter.BytesToHex(data);

                case DataFormatEnum.Ascii:
                    return FormatConverter.BytesToAscii(data);

                default:
                    throw new NotSupportedException();
            }
        }
    }
}
