
namespace ClientBlockChain.Interface;

public interface IReceive
{
    Task ReceiveDataAsync(CancellationToken cts = default);
}
