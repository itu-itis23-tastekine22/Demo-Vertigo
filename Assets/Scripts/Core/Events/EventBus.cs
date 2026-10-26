using System;
using System.Collections.Generic;

namespace VertigoWheel.Events
{
    public sealed class EventBus : IEventBus
    {
        private readonly Dictionary<Type, Delegate> _handlers = new Dictionary<Type, Delegate>();

        public void Subscribe<T>(Action<T> handler) where T : struct
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            Type eventType = typeof(T);

            if (_handlers.TryGetValue(eventType, out Delegate existingHandlers))
            {
                _handlers[eventType] = Delegate.Combine(existingHandlers, handler);
            }
            else
            {
                _handlers[eventType] = handler;
            }
        }

        public void Unsubscribe<T>(Action<T> handler) where T : struct
        {
            Type eventType = typeof(T);

            if (!_handlers.TryGetValue(eventType, out Delegate existingHandlers))
            {
                return;
            }

            Delegate remainingHandlers = Delegate.Remove(existingHandlers, handler);

            if (remainingHandlers == null)
            {
                _handlers.Remove(eventType);
            }
            else
            {
                _handlers[eventType] = remainingHandlers;
            }
        }

        public void Publish<T>(T eventData) where T : struct
        {
            if (!_handlers.TryGetValue(typeof(T), out Delegate existingHandlers))
            {
                return;
            }

            Action<T> typedHandlers = (Action<T>)existingHandlers;
            typedHandlers.Invoke(eventData);
        }
    }
}