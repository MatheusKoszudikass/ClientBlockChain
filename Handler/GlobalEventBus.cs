using System.Collections.Concurrent;
using ClientBlockChain.Entities;

namespace ClientBlockChain.Handler;

public class GlobalEventBus
{
    private static GlobalEventBus? _instance;
    public static GlobalEventBus InstanceValue => _instance ??= new GlobalEventBus();

    private readonly ConcurrentDictionary<Type, List<object>> Handlers = new();

    private GlobalEventBus() { }

    public void Subscribe<T>(Action<T> handler)
    {
        var type = typeof(T);
        if (!Handlers.ContainsKey(type))
        {
            Handlers[type] = [];
        }

        lock (Handlers[type])
        {
            Handlers[type].Add(handler);
        }
    }

    public void SubscribeList<T>(Action<List<T>> handlers)
    {
        var type = typeof(List<T>);
        if (!Handlers.ContainsKey(type))
        {
            Handlers[type] = [];
        }

        lock (Handlers[type])
        {
            Handlers[type].Add(handlers);
        }
    }

    public void Publish<T>(T eventData)
    {
        var type = typeof(T);
        if (!Handlers.TryGetValue(type, out var handlers)) return;
        foreach (var handler in handlers.ToList())
        {
            ((Action<T>)handler)(eventData);
        }
    }

    public void PublishList<T>(List<T> eventData)
    {
        var type = typeof(List<T>);
        if (!Handlers.TryGetValue(type, out var handlers)) return;
        foreach (var handler in handlers.ToList())
        {
            ((Action<List<T>>)handler)(eventData);
        }
    }

    public void Unsubscribe<T>(Action<T> handler)
    {
        var type = typeof(T);
        if (!Handlers.TryGetValue(type, out var handlers)) return;
        lock (handlers)
        {
            handlers.Remove(handler);
        }
    }

    public void UnsubscribeList<T>(Action<List<T>> handler)
    {
        var type = typeof(List<T>);
        if (!Handlers.TryGetValue(type, out var handlers)) return;

        lock (handlers)
        {
            handlers.Remove(handler);
        }
    }

    public static void ResetInstance()
    {
        var newInstance = new GlobalEventBus();
        _instance = newInstance;
    }
}

