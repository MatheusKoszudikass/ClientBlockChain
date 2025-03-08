using System.Net.Security;
using ServerBlockChain.Entities;
using ServerBlockChain.Interface;

namespace ServerBlockChain.Service
{
    public sealed class ReceiveService(SslStream ssltream,
        CancellationTokenSource cancellationToken) : IReceive
    {
        private readonly SslStream _sslStream = ssltream;
        private Receive? _receive;
        private readonly CancellationTokenSource _cancellationToken = cancellationToken;
        private readonly GlobalEventBus? _globalEventBus = GlobalEventBus.InstanceValue;
        public event Action<byte[]>? ReceivedAtc;
        public event Action<SslStream>? ClientDesconnectedAct;

        public async Task ReceiveDataAsync()
        {
          _receive = new Receive(_sslStream, _cancellationToken);

          _receive.ClientDisconnectedAct += (data) => ClientDesconnectedAct?.Invoke(data);
          _receive.ReceivedAct += OnReceivedAtc;

           await _receive.ReceiveDataAsync();
        }

        private void OnReceivedAtc(byte[] data)
        {
           _globalEventBus!.Publish(data);
        }
        
    }
}