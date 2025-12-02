using System.ComponentModel;
using System.Runtime.CompilerServices;
using ZConnect.Models;
using ZConnect.Utils;

namespace ZConnect.ViewModels
{
    public class BaseCommunicationViewModel : INotifyPropertyChanged    // Interface that notifies the UI that "a certain property has changed."
    {
        public event PropertyChangedEventHandler? PropertyChanged;  // Events are emitted by ViewModel, the UI automatically subscribes to the event through data binding.
        protected void OnPropertyChanged([CallerMemberName] string? name = null)  // [CallerMemberName] is a syntactic sugar for C#, it will automatically pass in the property name that calls the method.
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        protected Func<byte[], Task>? SendAction;   // C# 内置的委托类型，Func<byte[], Task> 表示接收 byte[] 类型的参数， 返回 Task 方法，简言之是一个可以异步调用的函数

        private string _status = "None";
        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
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

        public void ClearText()
        {
            ReceivedText = "";
        }

        public async Task SendAsync()
        {
            if (_isAutoSending || string.IsNullOrEmpty(SendText)) return;

            byte[] data = FormatConverter.ConvertSend(SendText, SendFormat);

            _isAutoSending = AutoSend;
            do
            {
                if (SendAction != null) await SendAction(data);

                ReceivedText += $"[Send {DateTime.Now:HH:mm:ss}] {FormatConverter.ConvertReceived(data, SendFormat)}\n";    // The recived/send text record.
                if (!AutoSend) break;
                await Task.Delay(IntervalMs).ConfigureAwait(false);
            } while (AutoSend);
            _isAutoSending = false;
        }

        protected void OnStatusChanged(object? sender, ICommunicationStatus args)
        {
            if (args.Data != null)
            {
                string text = FormatConverter.ConvertReceived(args.Data, ReceiveFormat);
                ReceivedText += $"[Recv {DateTime.Now:HH:mm:ss}] {text}\n";
            }

            Status = args.StatusType.ToString();
        }
    }
}
