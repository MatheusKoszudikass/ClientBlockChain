namespace ClientBlockChain.Interface;

public interface IDataMonitor<T>
{
    Task ReceiveDataAsync(CancellationToken cts = default);
    Task SendDataAsync(T data, CancellationToken cts = default);
    Task SendListDataAsync(List<T> listData,
     CancellationToken cts = default);
    void StopMonitoring();
}