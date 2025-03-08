using System.Net.Security;

namespace ClientBlockchain.Interface;
public interface IReceive<T>
{
    Task ReceiveDataAsync(SslStream sslStream,
        CancellationToken cancellationToken = default);

    Task ReceiveListDataAsync(SslStream sslStream,
        CancellationToken cancellationToken = default);
}