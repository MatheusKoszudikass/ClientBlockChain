using ClientBlockchain.Entities;
using ClientBlockChain.Entities.Enum;
using ClientBlockchain.Handler;
using ClientBlockchain.Interface;
using ClientBlockChain.Interface;

namespace ClientBlockChain.Service;
public sealed class ClientMineService : IClientMineService
{
    private readonly IDataMonitorService<ClientMine> _dataMonitorService;
    private GlobalEventBus _globalEventBus = GlobalEventBus.InstanceValue;
    private readonly IIlogger<ClientMineService> _ilogger;

    public ClientMineService(
        IDataMonitorService<ClientMine> dataMonitorService,
        IIlogger<ClientMineService> ilogger)
    {
        _dataMonitorService = dataMonitorService;
        _ilogger = ilogger;
        _globalEventBus.Subscribe<ClientMine>(OnClientMineInfo);
    }

    public async Task ClientMineInfoAsync(Listener listener,
     CancellationToken cts = default)
    {
        GlobalEventBusNewInstance();
        var clientMine = new ClientMine();
        // await _dataMonitorService.StartDepencenciesAsync(Listener.Instance, cts);

        if (!Listener.Instance.Listening || !AuthenticateServer.SslStream!.IsAuthenticated) return;

        await _dataMonitorService.SendDataAsync(clientMine, cts);
        _ = _ilogger.Log(clientMine, "ClientMineInfoAsync", LogLevel.Information);

    }

    private void GlobalEventBusNewInstance()
    {
        _globalEventBus = GlobalEventBus.InstanceValue;
    }
    private static void OnClientMineInfo(ClientMine data) => Console.WriteLine($"OnClientMineInfo{data.Name}. {data.Id}");
}
