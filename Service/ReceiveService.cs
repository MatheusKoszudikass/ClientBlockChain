using System.Net.Security;
using ClientBlockchain.Entities;
using ClientBlockChain.Entities;
using ClientBlockchain.Handler;
using ClientBlockchain.Interface;

namespace ClientBlockchain.Service
{
    public sealed class ReceiveService<T> : IReceive<T>
    {
        private Receive<T>? _receive;
        private ReceiveList<T>? _receiveList;
        private readonly ManagerTypeEventBus _managerTypeEventBus = new ManagerTypeEventBus();

        public async Task ReceiveDataAsync(SslStream sslStream, CancellationToken cts)
        {
            _receive = new Receive<T>(sslStream, cts);

            _receive.Received += OnReceivedAtc;

            await _receive.ReceiveDataAsync();
        }

        public async Task ReceiveListDataAsync(SslStream sslStream, CancellationToken cts)
        {
            _receiveList = new ReceiveList<T>(sslStream, cts);

            _receiveList.ReceiveListAct += OnReceiveListAtc;

            await _receiveList.ReceiveListAsync();
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