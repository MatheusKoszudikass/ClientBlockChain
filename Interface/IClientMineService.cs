using ClientBlockchain.Entities;

namespace ClientBlockChain.Interface;

public interface IClientMineService
{
    Task ClientMineInfoAsync(Listener listener, CancellationToken cts = default);
}