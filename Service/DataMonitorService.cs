using System.Net.Security;
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
    private SslStream? _sslStream;
    private T? _data { get; set; } = default!;

    public async Task StartDepencenciesAsync(Listener listener, CancellationToken cts)
    {
        try
        {
            GlobalEventBusNewInstance();
            _sslStream = await AuthenticateServer.AuthenticateAsClient(Listener.Instance.GetSocket(), cts);

            VerifyDependencies();
            _ = _ilogger.Log(_data!, "StartDependenciesAsync", LogLevel.Information);
        }
        catch (SocketException ex)
        {
            _ = _ilogger.Log(_data!, ex, ex.Message + "StartDependenciesAsync socket", LogLevel.Error);
            NotifyDisconnection();
            throw new Exception("StartDependenciesAsync socket");
        }
        catch (Exception ex)
        {
            _ = _ilogger.Log(_data!, ex, ex.Message +
                                     "StartDependenciesAsync exception", LogLevel.Error);
            NotifyDisconnection();
            throw new Exception("StartDependenciesAsync exception");
        }
    }

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
                    _ = _ilogger.Log(_data!, "ReceiveDataAsync", LogLevel.Information);
                    await _receive!.ReceiveDataAsync(_sslStream!, cts);
                    break;
                }
                catch (SocketException ex)
                {
                    _ = _ilogger.Log(_data!, ex, ex.Message + "ReceiveDataAsync socket", LogLevel.Error);
                    NotifyDisconnection();
                    throw new SocketException();
                }
                catch (Exception ex)
                {
                    _ = _ilogger.Log(_data!, ex, ex.Message + "ReceiveDataAsync exception", LogLevel.Error);
                    NotifyDisconnection();
                    throw new Exception("ReceiveDataAsync exception");
                }
            }
        }
        catch (SocketException ex)
        {
            _ = _ilogger.Log(_data!, ex, ex.Message + "ReceiveDataAsync socket", LogLevel.Error);
            NotifyDisconnection();
            throw new SocketException();
        }
        catch (Exception ex)
        {
            _ = _ilogger.Log(_data!, ex, ex.Message + "ReceiveDataAsync exception", LogLevel.Error);
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

            _ = _ilogger.Log(data, "SendDataAsync", LogLevel.Information);
            await _send!.SendAsync(data, _sslStream!, cts);
        }
        catch (SocketException ex)
        {
            _ = _ilogger.Log(_data!, ex, ex.Message + "SendDataAsync socket", LogLevel.Error);
            NotifyDisconnection();
            throw new SocketException();
        }
        catch (Exception ex)
        {
            _ = _ilogger.Log(_data!, ex, ex.Message + "SendDataAsync exception", LogLevel.Error);
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
            _ = _ilogger.Log(listData, "SendListDataAsync", LogLevel.Information);
            await _send!.SendListAsync(listData, _sslStream!, cts);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending data log: {ex.Message}");
            _ = _ilogger.Log(listData, ex, "Error sending list data", LogLevel.Error);
            NotifyDisconnection();
            throw;
        }
    }

    public void StopMonitoring()
    {
        VerifyDependencies();
        try
        {
            _ = _ilogger.Log(_data!, "StopMonitoring", LogLevel.Information);
        }
        catch (Exception ex)
        {
            _ = _ilogger.Log(_data!, ex, ex.Message + "StopMonitoring exception", LogLevel.Error);
            NotifyDisconnection();
            throw new Exception("StopMonitoring exception");
        }
    }

    private void VerifyDependencies()
    {
        if (Listener.Instance == null || _sslStream == null || _receive == null || _send == null)
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