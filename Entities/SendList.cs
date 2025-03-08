using System.Net.Security;
using System.Text.Json;
using ClientBlockChain.Entities;

namespace ClientBlockchain.Entities;

public class SendList<T>(SslStream sslStream, CancellationToken cancellationToken)
{
    private readonly CancellationToken _cancellation = cancellationToken;
    private readonly SslStream _sslStream = sslStream;
    public Action<List<T>>? _sentListAtc;
    
    public async Task SendListAsync(List<T> listData)
    {
        await SendLengthPrefix(listData);
        await SendObjectAsync(listData);
    }
    private async Task SendLengthPrefix(List<T> listData)
    {
        StateObject.BufferSend = BitConverter.GetBytes(JsonSerializer.SerializeToUtf8Bytes(listData).Length);
        await _sslStream.WriteAsync(StateObject.BufferSend, _cancellation);
    }

    private async Task SendObjectAsync(List<T> listData)
    {
        StateObject.BufferSend = JsonSerializer.SerializeToUtf8Bytes(listData);
        await _sslStream.WriteAsync(StateObject.BufferSend, _cancellation); 
        OnSentListAtc(listData);
    }

    private void OnSentListAtc(List<T> listData)
    {
        _sentListAtc!.Invoke(listData);
    }
}