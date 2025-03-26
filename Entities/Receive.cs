using System.Net.Security;
using System.Text;
using System.Text.Json;

namespace ClientBlockChain.Entities;

public sealed class Receive(SslStream sslStream)
{
    private readonly SslStream _sslStream = sslStream;
    private int _totalBytesReceived;
    private readonly StateObject Buffer = new();
    public event Action<JsonElement>? ReceivedAct;
    public event Action<List<JsonElement>>? OnReceivedListAct;

    public async Task ReceiveDataAsync(CancellationToken cts = default)
    {
        await ExecuteWithTimeout(() => ReceiveLengthPrefix(cts), TimeSpan.FromMinutes(1), cts);
        await ExecuteWithTimeout(() => ReceiveObject(cts), TimeSpan.FromMinutes(1), cts);

        DeserializeObject();
    }

    private async Task ReceiveLengthPrefix(CancellationToken cts = default)
    {
        _ = await this._sslStream.ReadAsync(this.Buffer.BufferInit, cts);

        this.Buffer.BufferSize = BitConverter.ToInt32(this.Buffer.BufferInit, 0);
        this.Buffer.IsList = this.Buffer.BufferInit[4] == 1;

        this.Buffer.BufferReceive = new byte[this.Buffer.BufferSize];
    }

    private async Task ReceiveObject(CancellationToken cts = default)
    {
        _totalBytesReceived = 0;
        while (_totalBytesReceived < this.Buffer.BufferSize)
        {
            var bytesRead = await _sslStream.ReadAsync(
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

        if (this.Buffer.IsList)
        {
            var resultList = JsonSerializer.Deserialize<List<JsonElement>>(jsonData);
            OnReceivedList(resultList!);
            return;
        }
        var resultObj = JsonSerializer.Deserialize<JsonElement>(jsonData);
        OnReceived(resultObj!);
    }

    private static async Task ExecuteWithTimeout(Func<Task> taskFunc, TimeSpan timeout
      , CancellationToken cts = default)
    {
        var timeoutTask = Task.Delay(timeout, cts);
        var task = taskFunc();

        if (await Task.WhenAny(task, timeoutTask) == timeoutTask)
        
            throw new TimeoutException("Operation timed out.");

        await task;
    }

    private void OnReceived(JsonElement data)
    {
        ReceivedAct?.Invoke(data);
    }

    private void OnReceivedList(List<JsonElement> data)
    {
        OnReceivedListAct!.Invoke(data);
    }
}

