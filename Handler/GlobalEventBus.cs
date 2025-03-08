using System.Collections.Concurrent;

namespace ServerBlockChain.Handler
{
    public class GlobalEventBus
    {
        private static readonly Lazy<GlobalEventBus> Instance = new(() => new GlobalEventBus());
        public static GlobalEventBus InstanceValue => Instance.Value;
        private readonly ConcurrentDictionary<Type, List<object>> _handlers = new();

        private GlobalEventBus() { }

        public void Subscribe<T>(Action<T> handler) where T : class
        {
            var type = typeof(T);
            if (!_handlers.ContainsKey(type))
            {
                _handlers[type] = [];
            }
            
            lock (_handlers[type])
            {
                _handlers[type].Add(handler);
            }
        }

        public void Publish<T>(T eventData) where T : class
        {
            if (eventData == null) return;

            var type = typeof(T);
            if (!_handlers.TryGetValue(type, out var handlers)) return;
            foreach (var handler in handlers.ToList())
            {
                ((Action<T>)handler)(eventData);
            }
        }

        public void Unsubscribe<T>(Action<T> handler) where T : class
        {
            var type = typeof(T);
            if (!_handlers.TryGetValue(type, out var handlers)) return;
            lock (handlers)
            {
                handlers.Remove(handler);
            }
        }
    }
}