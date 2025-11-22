using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using ZConnect.Models;
using ZConnect.Services;
using ZConnect.Models.Events;

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

        public TcpClientViewModel()
        {
            _service.StatusChanged += OnStatusChanged;  // Event subscription (event_name += method_name). Whenever _service.DataReceived is called, the OnDataReceived method is automatically called.
        }

        private void OnStatusChanged(object? sender, TcpStatusChangedEventArgs args)
        {
            if (args.StatusType == TcpStatusType.DataReceived && args.Data != null)
            {
                string text = Encoding.UTF8.GetString(args.Data);
                ReceivedText += $"[Recv {DateTime.Now:HH:mm:ss}] {text}\n";
            }

            if (args.StatusType == TcpStatusType.Connected)
                IsConnected = true;
            else if (args.StatusType == TcpStatusType.Disconnected || args.StatusType == TcpStatusType.Error)
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

            byte[] data = Encoding.UTF8.GetBytes(SendText);
            await _service.SendAsync(data);

            ReceivedText += $"[Send {DateTime.Now:HH:mm:ss}] {SendText}\n"; // The recived/send text record.
        }

        public void Disconnect()
        {
            _service.Disconnect();
        }
    }
}
