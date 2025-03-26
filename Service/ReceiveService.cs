using ClientBlockChain.Handler;
using ClientBlockChain.Entities;
using ClientBlockChain.Interface;
using System.Text.Json;

namespace ClientBlockChain.Service;

public sealed class ReceiveService : IReceive
{
    private readonly ManagerTypeEventBus _managerTypeEventBus = new();

    public async Task ReceiveDataAsync(CancellationToken cts = default)
    {
        await Task.Run(async () =>
        {
            while (!cts.IsCancellationRequested)
            {
                Console.WriteLine("Waiting for data...");
                var receive = new Receive(AuthenticateServer.SslStream!);

                receive.ReceivedAct += OnReceivedAtc;
                receive.OnReceivedListAct += OnReceiveListAtc;

                await receive.ReceiveDataAsync(cts);
            }
        }, cts);
    }

    private void OnReceivedAtc(JsonElement data)
    {
        Console.WriteLine($"Receive data: {data}");
        _managerTypeEventBus.PublishEventType(data!);
    }

    private void OnReceiveListAtc(List<JsonElement> listData)
    {
        Console.WriteLine($"Receive data: {listData}");
        _managerTypeEventBus.PublishListEventType(listData!);
    }
}
