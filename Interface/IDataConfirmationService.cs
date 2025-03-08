using ClientBlockchain.Entities;

namespace ClientBlockChain.Interface;

public interface IDataConfirmationService
{
    Task StartMonitoringAsync(Listener listener, CancellationToken cts = default);

    Task Monitoring(Listener listener, CancellationToken cts = default);
}