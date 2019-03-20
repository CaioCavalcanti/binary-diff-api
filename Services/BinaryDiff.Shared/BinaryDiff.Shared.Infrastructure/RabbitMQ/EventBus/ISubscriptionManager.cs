using BinaryDiff.Shared.Infrastructure.RabbitMQ.Events;
using System;
using System.Collections.Generic;

namespace BinaryDiff.Shared.Infrastructure.RabbitMQ.EventBus
{
    public interface ISubscriptionManager
    {
        bool IsEmpty { get; }

        void Clear();

        string GetEventKey<T>();

        event EventHandler<string> OnEventRemoved;

        event EventHandler<string> OnEventAdded;

        IEnumerable<Subscription> GetSubscriptionsForEvent(string eventName);

        void AddSubscription<T, TH>()
           where T : IntegrationEvent
           where TH : IIntegrationEventHandler<T>;

        void RemoveSubscription<T, TH>()
             where T : IntegrationEvent
             where TH : IIntegrationEventHandler<T>;

        bool HasSubscriptionsForEvent<T>()
            where T : IntegrationEvent;

        bool HasSubscriptionsForEvent(string eventName);
    }
}
