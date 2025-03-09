using System.Net.Security;
using ClientBlockchain.Entities;
using ClientBlockChain.Entities;
using ClientBlockchain.Handler;
using ClientBlockchain.Interface;

namespace ClientBlockchain.Service
{
    public sealed class ReceiveService<T> : IReceive<T>
    {
        private readonly ManagerTypeEventBus _managerTypeEventBus = new ManagerTypeEventBus();

        public async Task ReceiveDataAsync(SslStream sslStream, CancellationToken cts = default)
        {
            var receive = new Receive<T>(sslStream);

            receive.Received += OnReceivedAtc;

            await receive.ReceiveDataAsync(cts);
        }

        public async Task ReceiveListDataAsync(SslStream sslStream, CancellationToken cts = default)
        {
            var receiveList = new ReceiveList<T>(sslStream);

            receiveList.ReceiveListAct += OnReceiveListAtc;

            await receiveList.ReceiveListAsync(cts);
        }

        private void OnReceivedAtc(T data)
        {
            Console.WriteLine($"Receive data: {data}");
            _managerTypeEventBus.PublishEventType(data!);
        }

        private void OnReceiveListAtc(List<T> listData)
        {
            Console.WriteLine($"Receive data: {listData}");
            var objectList = listData.Cast<object>().ToList();
            _managerTypeEventBus.PublishEventType(objectList);
        }
    }
}