using System.Net.Security;
using System.Net.Sockets;
using System.Text.Json;
using ClientBlockChain.Entities;

namespace ClientBlockchain.Entities;

public class SendList<T>(SslStream sslStream)
{
    private readonly SslStream SslStream = sslStream;
    private readonly StateObject Buffer = new();
    public readonly Action<List<T>>? SentListAct;

    public async Task SendListAsync(List<T> listData, CancellationToken cts = default)
    {
        await ExecuteWithTimeout(() => SendLengthPrefix(listData, cts), TimeSpan.FromMinutes(5), cts);
        await ExecuteWithTimeout(() => SendObjectAsync(listData, cts), TimeSpan.FromMinutes(5), cts);
        await this.SslStream.FlushAsync(cts);
    }
    private async Task SendLengthPrefix(List<T> listData, CancellationToken cts = default)
    {
        this.Buffer.BufferSend = BitConverter.GetBytes(JsonSerializer.SerializeToUtf8Bytes(listData).Length);

        Array.Copy(this.Buffer.BufferSend, this.Buffer.BufferInit, 4);
        this.Buffer.BufferInit[4] = (byte)1;

        await this.SslStream.WriteAsync(this.Buffer.BufferInit, cts);
    }

    private async Task SendObjectAsync(List<T> listData, CancellationToken cts = default)
    {
        this.Buffer.BufferSend = JsonSerializer.SerializeToUtf8Bytes(listData);
        await this.SslStream.WriteAsync(this.Buffer.BufferSend, cts);
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
