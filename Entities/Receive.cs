using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ClientBlockChain.Entities
{
    public sealed class Receive<T>(SslStream sslStream, CancellationToken cancellationToken)
    {
        private readonly SslStream _sslStream = sslStream;
        private int _totalBytesReceived;
        private readonly CancellationToken _cancellationToken = cancellationToken;
        public event Action<T>? Received;
        public event Action<CancellationTokenSource>? CanceledOperation;

        public async Task ReceiveDataAsync()
        {
            try
            {
                await ExecuteWithTimeout( () =>ReceiveLengthPrefix(), TimeSpan.FromSeconds(5));
                await ExecuteWithTimeout(() => ReceiveObject(), TimeSpan.FromSeconds(5));

                await _sslStream.FlushAsync();
                DeserializeObject();
            }
            catch (SocketException ex)
            {
                throw new Exception($"Error receiving object: {ex.Message}");
            }
        }

        private async Task ReceiveLengthPrefix()
        {
             _ = await _sslStream.ReadAsync(StateObject.BufferInit, _cancellationToken);
            StateObject.BufferReceiveSize = BitConverter.ToInt32(StateObject.BufferInit, 0);
            StateObject.BufferReceive = new byte[StateObject.BufferReceiveSize];
        }

        private async Task ReceiveObject()
        {
            _totalBytesReceived = 0;
            while (_totalBytesReceived < StateObject.BufferReceiveSize)
            {
                var bytesRead = await _sslStream.ReadAsync(
                    StateObject.BufferReceive.AsMemory(_totalBytesReceived,
                    StateObject.BufferReceiveSize - _totalBytesReceived), _cancellationToken);

                if (bytesRead == 0) break;
                _totalBytesReceived += bytesRead;
            }
        }

        private void DeserializeObject()
        {
            try
            {
                if (this._totalBytesReceived != StateObject.BufferReceiveSize) return;
                var jsonData = Encoding.UTF8.GetString(StateObject.BufferReceive, 0, _totalBytesReceived);
                var resultObj = JsonSerializer.Deserialize<T>(jsonData);
                OnReceived(resultObj!);
            }
            catch (JsonException ex)
            {

                throw new Exception($"Error deserializing object: {ex.Message}");
            }
        }

        private async Task ExecuteWithTimeout(Func<Task> taskFunc, TimeSpan timeout)
        {
            var timeoutTask = Task.Delay(timeout, _cancellationToken);
            var task = taskFunc();

            if (await Task.WhenAny(task, timeoutTask) == timeoutTask)
                throw new TimeoutException("Operation timed out.");

            await task;
        }
        
        private void OnReceived(T data)
        {
            Received?.Invoke(data);
        }

        private void OnCanceledOperation(CancellationTokenSource cancellationTokenSource)
        {
            CanceledOperation?.Invoke(cancellationTokenSource);
        }
    }
}