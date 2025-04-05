using ClientBlockChain.Entities;

namespace ClientBlockChain.Interface;

public interface IEventApp<T>
{
    event Action<T>? Received;
    event Action<T>? Sent;
    event Action<T>? SentLog;
    event Action<Listener>? ServerDisconnected;
    event Action<Listener>? ServerConnected;
    event Action<T>? StatusChanged;

    void OnReceived(T data);
    void OnSent(T data);
    void OnSentLog(T data);
    void OnServerDisconnected(Listener listener);
    void OnServerConnected(Listener listener);
    void OnStatusChanged(T data);
}