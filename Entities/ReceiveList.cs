using System.Net.Security;
using System.Text;
using System.Text.Json;
using ClientBlockChain.Entities;

namespace ClientBlockchain.Entities;

public class ReceiveList<T>(SslStream sslStream)
{
    private readonly SslStream SslStream = sslStream;
    private readonly StateObject Buffer = new();

    public event Action<List<T>>? ReceiveListAct;
    private int _totalBytesReceived;

    public async Task ReceiveListAsync(CancellationToken cts = default)
    {
        await ExecuteWithTimeout(() => ReceiveLengthPrefix(cts), TimeSpan.FromSeconds(5), cts);
        await ExecuteWithTimeout(() => ReceiveObjectAsync(cts), TimeSpan.FromSeconds(5), cts);
        await SslStream.FlushAsync(cts);
        DeserializeObject();
    }

    private async Task ReceiveLengthPrefix(CancellationToken cts = default)
    {
        _ = await SslStream.ReadAsync(this.Buffer.BufferInit, cts);
        this.Buffer.BufferSize = BitConverter.ToInt32(this.Buffer.BufferInit, 0);
        this.Buffer.BufferReceive = new byte[this.Buffer.BufferSize];
    }

    private async Task ReceiveObjectAsync(CancellationToken cts = default)
    {
        _totalBytesReceived = 0;
        while (_totalBytesReceived < this.Buffer.BufferSize)
        {
            var bytesRead = await SslStream.ReadAsync(
                this.Buffer.BufferReceive.AsMemory(_totalBytesReceived,
                    this.Buffer.BufferSize - _totalBytesReceived), cts);
            if (bytesRead == 0) break;
            _totalBytesReceived += bytesRead;
        }
    }

    private void DeserializeObject()
    {
        if (this._totalBytesReceived != this.Buffer.BufferSize) return;
        var jsonData = Encoding.UTF8.GetString(this.Buffer.BufferReceive, 0, _totalBytesReceived);
        var resultObj = JsonSerializer.Deserialize<List<T>>(jsonData);
        OnReceiveList(resultObj!);

    }

    private static async Task ExecuteWithTimeout(Func<Task> taskFunc, TimeSpan timeout, CancellationToken cts)
    {
        var timeoutTask = Task.Delay(timeout, cts);
        var task = taskFunc();

        if (await Task.WhenAny(task, timeoutTask) == timeoutTask)
            throw new TimeoutException("Operation timed out.");

        await task;
    }

    private void OnReceiveList(List<T> listData) => ReceiveListAct!.Invoke(listData);
}