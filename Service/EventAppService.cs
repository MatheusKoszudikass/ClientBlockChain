using ClientBlockchain.Entities;
using ClientBlockChain.Interface;

namespace ClientBlockChain.Service;

public class EventAppService<T> : IEventApp<T>
{
    public event Action<T>? Received;
    public event Action<T>? Sent;
    public event Action<T>? SentLog;
    public event Action<Listener>? ServerDisconnected;
    public event Action<Listener>? ServerConnected;
    public event Action<T>? StatusChanged;

    public void OnReceived(T data)
    {
        Received?.Invoke(data);
    }

    public void OnSent(T data)
    {
        Sent?.Invoke(data);
    }

    public void OnSentLog(T data)
    {
        SentLog?.Invoke(data);
    }

    public void OnServerDisconnected(Listener listener)
    {
        ServerDisconnected?.Invoke(listener);
    }

    public void OnServerConnected(Listener listener)
    {
        ServerConnected?.Invoke(listener);
    }

    public void OnStatusChanged(T data)
    {
        StatusChanged?.Invoke(data);
    }
}
