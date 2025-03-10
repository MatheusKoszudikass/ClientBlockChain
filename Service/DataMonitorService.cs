using System.Net.Sockets;
using ClientBlockchain.Entities;
using ClientBlockChain.Entities.Enum;
using ClientBlockchain.Handler;
using ClientBlockchain.Interface;

namespace ClientBlockchain.Service;

public class DataMonitorService<T>(
    IIlogger<T> _ilogger,
    IReceive<T> receive,
    ISend<T> send) : IDataMonitorService<T>
{
    private readonly IReceive<T>? _receive = receive;
    private readonly ISend<T>? _send = send;
    private GlobalEventBus _globalEventBus = GlobalEventBus.InstanceValue;
    private T? _data { get; set; } = default!;

    public async Task ReceiveDataAsync(CancellationToken cts)
    {
        try
        {
            VerifyDependencies();
            GlobalEventBusNewInstance();

            while (!cts.IsCancellationRequested)
            {
                try
                {
                    await _ilogger.Log(_data!, "ReceiveDataAsync", LogLevel.Information);
                    await _receive!.ReceiveDataAsync(AuthenticateServer.SslStream!, cts);
                    break;
                }
                catch (SocketException ex)
                {
                    await _ilogger.Log(_data!, ex, ex.Message + "ReceiveDataAsync socket", LogLevel.Error);
                    NotifyDisconnection();
                    throw new SocketException();
                }
                catch (Exception ex)
                {
                    await _ilogger.Log(_data!, ex, ex.Message + "ReceiveDataAsync exception", LogLevel.Error);
                    NotifyDisconnection();
                    throw new Exception("ReceiveDataAsync exception");
                }
            }
        }
        catch (SocketException ex)
        {
            await _ilogger.Log(_data!, ex, ex.Message + "ReceiveDataAsync socket", LogLevel.Error);
            NotifyDisconnection();
            throw new SocketException();
        }
        catch (Exception ex)
        {
            await _ilogger.Log(_data!, ex, ex.Message + "ReceiveDataAsync exception", LogLevel.Error);
            NotifyDisconnection();
            throw new Exception("ReceiveDataAsync exception");
        }
    }

    public async Task SendDataAsync(T data, CancellationToken cts)
    {
        VerifyDependencies();
        GlobalEventBusNewInstance();

        try
        {
            Console.WriteLine($"Send data: {data}");

            await _ilogger.Log(data, "SendDataAsync", LogLevel.Information);
            await _send!.SendAsync(data, AuthenticateServer.SslStream!, cts);
        }
        catch (SocketException ex)
        {
            await _ilogger.Log(_data!, ex, ex.Message + "SendDataAsync socket", LogLevel.Error);
            NotifyDisconnection();
            throw new SocketException();
        }
        catch (Exception ex)
        {
            await _ilogger.Log(_data!, ex, ex.Message + "SendDataAsync exception", LogLevel.Error);
            NotifyDisconnection();
            throw new Exception("SendDataAsync exception");
        }
    }

    public async Task SendListDataAsync(List<T> listData, CancellationToken cts)
    {
        VerifyDependencies();
        GlobalEventBusNewInstance();

        try
        {
            await _ilogger.Log(listData, "SendListDataAsync", LogLevel.Information);
            await _send!.SendListAsync(listData, AuthenticateServer.SslStream!, cts);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending data log: {ex.Message}");
            await _ilogger.Log(listData, ex, "Error sending list data", LogLevel.Error);
            NotifyDisconnection();
            throw;
        }
    }

    public void StopMonitoring()
    {
        VerifyDependencies();
        try
        {
            _= _ilogger.Log(_data!, "StopMonitoring", LogLevel.Information);
        }
        catch (Exception ex)
        {
            _= _ilogger.Log(_data!, ex, ex.Message + "StopMonitoring exception", LogLevel.Error);
            NotifyDisconnection();
            throw new Exception("StopMonitoring exception");
        }
    }

    private void VerifyDependencies()
    {
        if (Listener.Instance == null || AuthenticateServer.SslStream == null || _receive == null || _send == null)
        {
            _ilogger.Log(_data!, "Dependencies not initialized", LogLevel.Error);
            NotifyDisconnection();
            throw new Exception("Dependencies not initialized");
        }
    }

    private void GlobalEventBusNewInstance()
    {
        _globalEventBus = GlobalEventBus.InstanceValue;

    }

    private void NotifyDisconnection()
    {
        _globalEventBus.Publish(Listener.Instance);
    }
}