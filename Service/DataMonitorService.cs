using System.Net.Sockets;
using ClientBlockChain.Entities.Enum;
using ClientBlockChain.Handler;
using ClientBlockChain.Interface;
using ClientBlockChain.Entities;

namespace ClientBlockChain.Service;

public class DataMonitorService<T>(
    IIlogger<T> _ilogger,
    IReceive receive,
    ISend<T> send) : IDataMonitor<T>
{
    private readonly IReceive? _receive = receive;
    private readonly ISend<T>? _send = send;
    private readonly GlobalEventBus _globalEventBus = GlobalEventBus.InstanceValue;
    private T? _data { get; set; } = default!;

    public async Task ReceiveDataAsync(CancellationToken cts = default)
    {
        try
        {
            while (!cts.IsCancellationRequested)
            {
                try
                {
                    await _ilogger.Log(_data!, "ReceiveDataAsync", LogLevel.Information);
                    await _receive!.ReceiveDataAsync(cts);
                    break;
                }
                catch (SocketException ex)
                {
                    await _ilogger.Log(_data!, ex, ex.Message + "ReceiveDataAsync socket", LogLevel.Error);
                    NotifyDisconnection();
                }
                catch (Exception ex)
                {
                    await _ilogger.Log(_data!, ex, $"{ex.Message} ReceiveDataAsync exception", LogLevel.Error);
                    NotifyDisconnection();
                }
            }
        }
        catch (SocketException ex)
        {
            await _ilogger.Log(_data!, ex, ex.Message + "ReceiveDataAsync socket", LogLevel.Error);
            NotifyDisconnection();
        }
        catch (Exception ex)
        {
            await _ilogger.Log(_data!, ex, $"{ex.Message} ReceiveDataAsync exception", LogLevel.Error);
            NotifyDisconnection();
        }
    }

    public async Task SendDataAsync(T data, CancellationToken cts = default)
    {
        try
        {
            Console.WriteLine($"Send data: {data}");

            await _ilogger.Log(data, "SendDataAsync", LogLevel.Information);
            await _send!.SendAsync(data, cts);
        }
        catch (SocketException ex)
        {
            await _ilogger.Log(_data!, ex, ex.Message + "SendDataAsync socket", LogLevel.Error);
            NotifyDisconnection();
        }
        catch (Exception ex)
        {
            await _ilogger.Log(_data!, ex, ex.Message + "SendDataAsync exception", LogLevel.Error);
            NotifyDisconnection();
        }
    }

    public async Task SendListDataAsync(List<T> listData, CancellationToken cts = default)
    {
        try
        {
            await _ilogger.Log(listData, "SendListDataAsync", LogLevel.Information);
            await _send!.SendListAsync(listData!, cts);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending data log: {ex.Message}");
            await _ilogger.Log(listData, ex, "Error sending list data", LogLevel.Error);
            NotifyDisconnection();
        }
    }

    public void StopMonitoring()
    {
        try
        {
            _ = _ilogger.Log(_data!, "StopMonitoring", LogLevel.Information);
        }
        catch (Exception ex)
        {
            _ = _ilogger.Log(_data!, ex, ex.Message + "StopMonitoring exception", LogLevel.Error);
            NotifyDisconnection();
        }
    }

    private void NotifyDisconnection()
    {
        _globalEventBus.Publish(Listener.Instance);
    }
}