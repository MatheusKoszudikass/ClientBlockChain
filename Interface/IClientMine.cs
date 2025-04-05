
namespace ClientBlockChain.Interface;
public interface IClientMine
{
    Task ClientMineInfoAsync(CancellationToken cts = default);
}