using ClientBlockChain.Entities;
using ClientBlockChain.Entities.Enum;
using ClientBlockChain.Interface;

namespace ClientBlockChain.Service;
public sealed class ClientMineService(
    IDataMonitor<ClientMine> dataMonitorService,
    IIlogger<ClientMineService> ilogger) : IClientMine
{
    private readonly IDataMonitor<ClientMine> _dataMonitorService = dataMonitorService;
    private readonly IIlogger<ClientMineService> _ilogger = ilogger;

    public async Task ClientMineInfoAsync(
     CancellationToken cts = default)
    {
        var clientMine = new ClientMine();

        if (!Listener.Instance.Listening || !AuthenticateServer.SslStream!.IsAuthenticated) return;

        await _dataMonitorService.SendDataAsync(clientMine, cts);
        _ = _ilogger.Log(clientMine, "ClientMineInfoAsync", LogLevel.Information);
    }
}
