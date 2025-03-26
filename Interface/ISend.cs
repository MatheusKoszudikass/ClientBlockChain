namespace ClientBlockChain.Interface;

public interface ISend<T>
{
    Task SendAsync(T data,
    CancellationToken cts = default);

    Task SendListAsync(List<T> listData,
        CancellationToken cts = default);
}
