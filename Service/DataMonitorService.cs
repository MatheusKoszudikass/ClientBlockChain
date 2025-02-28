using System.Net.Security;
using ClientBlockchain.Entities;
using ClientBlockChain.Entities;
using ClientBlockChain.Entity;
using ClientBlockChain.Interface;
using System.Net.Sockets;

namespace ClientBlockChain.Service
{
    public class DataMonitorService<T> : IDataMonitorService<T>
    {
        private Listener? _listener;
        private SslStream? _auth;
        private Send<T>? _send;
        private Receive<T>? _receive;
        private T? _data;
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public async Task StartDepencenciesAsync(Listener listener)
        {
            try
            {
                _listener = listener;
                _auth = await AuthenticateServer.AuthenticateAsClient(listener.GetSocket());
                _receive = new Receive<T>(_auth, _cancellationTokenSource);
                _send = new Send<T>(_auth, _cancellationTokenSource);

                VerifyDependencies();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during monitoring: {ex.Message}");
                throw;
            }
        }

        public async Task ReceiveDataAsync()
        {
            VerifyDependencies();
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    _receive!.Received += (sender, data) =>
                        Console.WriteLine($"Received data: {data}");

                    await _receive.ReceiveDataAsync();
                    break;
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"listener error: {ex.Message}");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error receiving data: {ex.Message}");
                    break;
                }
            }
        }

        public async Task SendDataAsync(T data)
        {
            VerifyDependencies();
            _data = data;
            _send!.Sending += (sender, data) => Console.WriteLine($"Sent data: {data}");
            await _send.SendAsync(_data!);
        }

        public void StopMonitoring()
        {
            _cancellationTokenSource.Cancel();
        }

        private void VerifyDependencies()
        {
            if (_listener == null || _auth == null || _receive == null || _send == null)
                throw new Exception("Dependencies not initialized");
        }
    }
}