using System.Collections.Concurrent;
using ClientBlockChain.Entities;

namespace ClientBlockchain.Handler
{
    public class GlobalEventBus
    {
        private static GlobalEventBus? _instance;
        public static GlobalEventBus InstanceValue => _instance ??= new GlobalEventBus();

        private ConcurrentDictionary<Type, List<object>> Handlers = new();

        private GlobalEventBus() { }

        public void Subscribe<T>(Action<T> handler) where T : class
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

        public void SubscribeList<T>(Action<List<T>> handlers) where T : class
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

        public void Publish<T>(T eventData) where T : class
        {
            var type = typeof(T);
            if (!Handlers.TryGetValue(type, out var handlers)) return;
            foreach (var handler in handlers.ToList())
            {
                ((Action<T>)handler)(eventData);
            }
        }

        public void PublishList<T>(List<T> eventData) where T : class
        {
            var type = typeof(List<T>);
            if (!Handlers.TryGetValue(type, out var handlers)) return;
            foreach (var handler in handlers.ToList())
            {
                ((Action<List<T>>)handler)(eventData);
            }
        }

        public void Unsubscribe<T>(Action<T> handler) where T : class
        {
            var type = typeof(T);
            if (!Handlers.TryGetValue(type, out var handlers)) return;
            lock (handlers)
            {
                handlers.Remove(handler);
            }
        }

        public void UnsubscribeList<T>(Action<List<T>> handler) where T : class
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
            var oldHandlers = InstanceValue.Handlers;
            var newInstance = new GlobalEventBus();

            var logEntryType = typeof(List<LogEntry>);
            
            if (oldHandlers.TryGetValue(logEntryType, out var logEntryHandlers))
            {
                var handlersToRemove = logEntryHandlers.Cast<Action<List<LogEntry>>>().ToList();
                foreach (var handler in handlersToRemove)
                {
                    InstanceValue.UnsubscribeList(handler);
                    newInstance.SubscribeList(handler);
                }
            }

            InstanceValue.Handlers.Clear();
            _instance = newInstance;
        }

    }
}
