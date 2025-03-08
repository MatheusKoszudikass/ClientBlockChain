using System.Net.Security;
using ClientBlockchain.Entities;
using ClientBlockChain.Entities;
using ClientBlockchain.Handler;
using ClientBlockchain.Interface;

namespace ClientBlockchain.Service
{
    public class SendService<T> : ISend<T>
    {
        private readonly ManagerTypeEventBus _managerTypeEventBus = new();

        public async Task SendAsync(T data, SslStream sslStream,
            CancellationToken cts = default)
        {
            var send = new Send<T>(sslStream);
            send.SentAct += OnSendingAtc;

            if(data is List<T> listData)
            {
                await send!.SendListAsync(listData, cts);
            }
            await send.SendAsync(data, cts);
        }

        public async Task SendListAsync(List<T> listData, SslStream sslStream,
            CancellationToken cts = default)
        {
            var sendList = new SendList<T>(sslStream);

            await sendList.SendListAsync(listData, cts);
        }

        private void OnSendingAtc(T data)
        {
            Console.WriteLine($"Send data: {data}");
            _managerTypeEventBus.PublishEventType(data!);
        }
    }
}