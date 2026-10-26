using System;

namespace VertigoWheel.Events
{
    public interface IEventBus
    {
        void Subscribe<T>(Action<T> handler) where T : struct;

        void Unsubscribe<T>(Action<T> handler) where T : struct;

        void Publish<T>(T eventData) where T : struct;
    }
}