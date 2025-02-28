using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClientBlockChain.Entities
{
    public class Send<T> (SslStream sslStream, CancellationTokenSource cancellationTokenSource)
    {
        private readonly SslStream _sslStream =  sslStream;
        private readonly CancellationTokenSource _cancellationTokenSource = cancellationTokenSource;
        public event EventHandler<T>? Sending;
        public event EventHandler<T>? Sent;
        

        public async Task SendAsync(T data)
        {
            try
            {
                // await ExecuteWithTimeout( () => SendLengthPrefix(data), TimeSpan.FromSeconds(5));
                // await ExecuteWithTimeout( () => SendObject(data), TimeSpan.FromSeconds(5));

                await SendLengthPrefix(data);
                await SendObject(data);
            }
            catch (SocketException ex)
            {
                throw new Exception($"Error sending object: {ex.Message}");
            }
        }

        private async Task SendLengthPrefix(T data)
        {
            StateObject.BufferSend = BitConverter.GetBytes(JsonSerializer.SerializeToUtf8Bytes(data).Length);
            await _sslStream.WriteAsync(StateObject.BufferSend, _cancellationTokenSource.Token);
        }

        private async Task SendObject(T data)
        {
            StateObject.BufferSend = JsonSerializer.SerializeToUtf8Bytes(data);
            await _sslStream.WriteAsync(StateObject.BufferSend, _cancellationTokenSource.Token);
            OnSending(data);
        }

        private async Task ExecuteWithTimeout(Func<Task> taskFunc, TimeSpan timeout)
        {
            var timeoutTask = Task.Delay(timeout, _cancellationTokenSource.Token);
            var task = taskFunc();

            if(await Task.WhenAny(task, timeoutTask) == timeoutTask)
                throw new TimeoutException("Operation timed out.");

            await task;
        }

        protected virtual void OnSending(T data)
        {
            Sending?.Invoke(this, data);
        }

        protected virtual void OnSent(T data)
        {
            Sent?.Invoke(this, data);
        }
    }
}