using System.Net.Security;

namespace ClientBlockchain.Interface;

public interface ISend<T>
{
    Task SendAsync(T data, SslStream sslStream,
        CancellationToken cts = default);

    Task SendListAsync(List<T> listData, SslStream sslStream,
        CancellationToken cts = default);
}