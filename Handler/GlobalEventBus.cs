using System.Collections.Concurrent;
using ClientBlockChain.Entities;

namespace ClientBlockchain.Handler
{
    public class GlobalEventBus
    {
        private static GlobalEventBus? _instance;
        public static GlobalEventBus InstanceValue => _instance ??= new GlobalEventBus();

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

        public void SubscribeList<T>(Action<List<T>> handlers) where T : class
        {
            var type = typeof(List<T>);
            if (!_handlers.ContainsKey(type))
            {
                _handlers[type] = [];
            }

            lock (_handlers[type])
            {
                _handlers[type].Add(handlers);
            }
        }

        public void Publish<T>(T eventData) where T : class
        {
            var type = typeof(T);
            if (!_handlers.TryGetValue(type, out var handlers)) return;
            foreach (var handler in handlers.ToList())
            {
                ((Action<T>)handler)(eventData);
            }
        }

        public void PublishList<T>(List<T> eventData) where T : class
        {
            var type = typeof(List<T>);
            if (!_handlers.TryGetValue(type, out var handlers)) return;
            foreach (var handler in handlers.ToList())
            {
                ((Action<List<T>>)handler)(eventData);
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

        public static void ResetInstance()
        {
            var oldHandlers = InstanceValue._handlers;
            var newInstance = new GlobalEventBus();

            var logEntryType = typeof(List<LogEntry>);

            if (oldHandlers.TryGetValue(logEntryType, out var logEntryHandlers))
            {
                newInstance._handlers[logEntryType] = new List<object>(logEntryHandlers);
            }
            InstanceValue._handlers.Clear();
            _instance = newInstance;
        }

    }
}
