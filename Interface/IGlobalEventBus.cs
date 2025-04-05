namespace ClientBlockChain.Interface;

public interface IGlobalEventBus
{
    void Subscribe<T>(Action<T> handler) where T : class;
    void Publish<T>(T message) where T : class;
    void Unsubscribe<T>(Action<T> handler) where T : class;
}