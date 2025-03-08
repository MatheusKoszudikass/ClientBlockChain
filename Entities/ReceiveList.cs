using System.Net.Security;
using System.Text;
using System.Text.Json;
using ClientBlockChain.Entities;

namespace ClientBlockchain.Entities;

public class ReceiveList<T>(SslStream sslStream, CancellationToken cancellationToken)
{
    private readonly SslStream _sslStream = sslStream;
    private readonly CancellationToken _cancellationToken = cancellationToken;
    public Action<List<T>>? ReceiveListAct;
    private int _totalBytesReceived;

    public async Task ReceiveListAsync()
    {
        await ExecuteWithTimeout(() => ReceiveLengthPrefix(), TimeSpan.FromSeconds(5), cts: _cancellationToken);
        await ExecuteWithTimeout(() => ReceiveObjectAsync(), TimeSpan.FromSeconds(5), cts: _cancellationToken);
        await _sslStream.FlushAsync();
        DeserializeObject();
    }

    private async Task ReceiveLengthPrefix()
    {
        _ = await _sslStream.ReadAsync(StateObject.BufferInit, _cancellationToken);
        StateObject.BufferReceiveSize = BitConverter.ToInt32(StateObject.BufferInit, 0);
        StateObject.BufferReceive = new byte[StateObject.BufferReceiveSize];
    }

    private async Task ReceiveObjectAsync()
    {
        _totalBytesReceived = 0;
        while (_totalBytesReceived < StateObject.BufferReceiveSize)
        {
            var bytesRead = await _sslStream.ReadAsync(
                StateObject.BufferReceive.AsMemory(_totalBytesReceived,
                    StateObject.BufferReceiveSize - _totalBytesReceived), _cancellationToken);
            if (bytesRead == 0) break;
            _totalBytesReceived += bytesRead;
        }
    }

    private void DeserializeObject()
    {
        try
        {
            if (this._totalBytesReceived != StateObject.BufferReceiveSize) return;
            var jsonData = Encoding.UTF8.GetString(StateObject.BufferReceive, 0, _totalBytesReceived);
            var resultObj = JsonSerializer.Deserialize<List<T>>(jsonData);
            OnReceiveList(resultObj!);
        }
        catch (JsonException ex)
        {
            throw new Exception($"Error deserializing object: {ex.Message}");
        }
    }

    private async Task ExecuteWithTimeout(Func<Task> taskFunc, TimeSpan timeout, CancellationToken cts)
    {
        var timeoutTask = Task.Delay(timeout, cts);
        var task = taskFunc();

        if (await Task.WhenAny(task, timeoutTask) == timeoutTask)
            throw new TimeoutException("Operation timed out.");

        await task;
    }

    private void OnReceiveList(List<T> listData) => ReceiveListAct!.Invoke(listData);
}