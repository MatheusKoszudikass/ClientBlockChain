using System.Net.Security;
using ServerBlockChain.Entities;
using ServerBlockChain.Interface;

namespace ServerBlockChain.Service
{
    public class SendService(SslStream sslStream, CancellationTokenSource cancellationToken) : ISend
    {
        private readonly SslStream _sslStream = sslStream;
        private readonly CancellationTokenSource _cancellationToken = cancellationToken;
        private Send? _send;
        public event Action<byte[]>? SendingAtc;
        public event Action<SslStream>? ClientDisconnectedAtc;
        
        public async Task SendAsync(byte[] data)
        {
            _send = new Send(_sslStream, _cancellationToken);
            _send.ClientDisconnectedAtc += (dataByte) => ClientDisconnectedAtc?.Invoke(dataByte);
            _send.SendingAtc += (dataByte) => SendingAtc?.Invoke(dataByte);

            await _send.SendAsync(data);
        }
    }
}