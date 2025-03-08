using System.Net.Security;
using System.Net.Sockets;
using System.Text.Json;
using ClientBlockChain.Entities;

namespace ClientBlockchain.Entities;

public class SendList<T>(SslStream sslStream)
{
    private readonly SslStream _sslStream = sslStream;
    public readonly Action<List<T>>? _sentListAtc;

    public async Task SendListAsync(List<T> listData, CancellationToken cts = default)
    {
        await ExecuteWithTimeout(() => SendLengthPrefix(listData, cts), TimeSpan.FromSeconds(5), cts);
        await ExecuteWithTimeout(() => SendObjectAsync(listData, cts), TimeSpan.FromSeconds(5), cts);
        await _sslStream.FlushAsync(cts);
    }
    private async Task SendLengthPrefix(List<T> listData, CancellationToken cts = default)
    {
        StateObject.BufferSend = BitConverter.GetBytes(JsonSerializer.SerializeToUtf8Bytes(listData).Length);

        Array.Copy(StateObject.BufferSend, StateObject.BufferInit, 4);
        StateObject.BufferInit[4] = (byte)1;

        await _sslStream.WriteAsync(StateObject.BufferInit, cts);
    }

    private async Task SendObjectAsync(List<T> listData, CancellationToken cts = default)
    {
        StateObject.BufferSend = JsonSerializer.SerializeToUtf8Bytes(listData);
        await _sslStream.WriteAsync(StateObject.BufferSend, cts);
    }


    private static async Task ExecuteWithTimeout(Func<Task> taskFunc, TimeSpan timeout, CancellationToken cts)
    {
        var timeoutTask = Task.Delay(timeout, cts);
        var task = taskFunc();

        if (await Task.WhenAny(task, timeoutTask) == timeoutTask)
            throw new TimeoutException("Operation timed out.");

        await task;
    }
}
