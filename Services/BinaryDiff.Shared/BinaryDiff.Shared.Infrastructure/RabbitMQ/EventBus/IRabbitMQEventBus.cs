using BinaryDiff.Shared.Infrastructure.RabbitMQ.Events;
using System;

namespace BinaryDiff.Shared.Infrastructure.RabbitMQ.EventBus
{
    public interface IRabbitMQEventBus : IDisposable
    {
        void Publish<TEvent>(TEvent @event)
            where TEvent : IntegrationEvent;

        void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;
    }
}
