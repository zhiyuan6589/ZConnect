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
    /// <summary>
    /// Tcp ViewModel base class.
    /// </summary>
    public abstract class BaseTcpViewMode : INotifyPropertyChanged    // Interface that notifies the UI that "a certain property has changed."
    {
        // MVVM Mode
        public event PropertyChangedEventHandler? PropertyChanged;  // Events are emitted by ViewModel, the UI automatically subscribes to the event through data binding.
        protected void OnPropertyChanged([CallerMemberName] string? name = null)  // [CallerMemberName] is a syntactic sugar for C#, it will automatically pass in the property name that calls the method.
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        protected readonly ITcpServiec _service;    // Any object that implements the ITcpService interface (TcpClientService or TcpServerService).

        public BaseTcpViewMode(ITcpServiec service)
        {
            _service = service;
        }

        public TcpConnectionModel Connection => _service.Connection;   // Read-only property, equivalent to: get { return _service.Connect; }, the function to expose `read-only` access to _service.Connection.

        private string _tcpStatus = "DisConnection";
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

        protected void OnStatusChanged(object? sender, TcpStatusChangedEventArgs args)
        {
            if (args.StatusType == TcpStatusEnum.DataReceived && args.Data != null)
            {
                string text = FormatConverter.ConvertReceived(args.Data, ReceiveFormat);
                ReceivedText += $"[Recv {DateTime.Now:HH:mm:ss}] {text}\n";
            }

            TcpStatus = args.StatusType.ToString();
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
                if (!AutoSend || !Connection.IsConnected) break;
                await Task.Delay(IntervalMs);
            } while (AutoSend && (Connection.IsConnected));
            _isAutoSending = false;
        }

        public void ClearText()
        {
            ReceivedText = "";
        }
    }
}
