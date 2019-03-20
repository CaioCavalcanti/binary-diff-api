using BinaryDiff.Shared.Infrastructure.RabbitMQ.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BinaryDiff.Shared.Infrastructure.RabbitMQ.EventBus
{
    /// <summary>
    /// In memory subscription manager that handles events and
    /// theirs subscriptions
    /// </summary>
    public class SubscriptionManager : ISubscriptionManager
    {
        private readonly IDictionary<string, List<Subscription>> _eventHandlers;

        public SubscriptionManager()
        {
            _eventHandlers = new Dictionary<string, List<Subscription>>();
        }

        public event EventHandler<string> OnEventRemoved;

        public event EventHandler<string> OnEventAdded;

        public bool IsEmpty => !_eventHandlers.Keys.Any();

        public void Clear() => _eventHandlers.Clear();

        public string GetEventKey<T>() => typeof(T).Name;

        /// <summary>
        /// Adds a subscription for a given event of type T (IntegrationEvent)
        /// with its handler of type TH (IntegrationEventHandler<T>) and
        /// raises OnEventAdded so EventBus can handle its queue creation.
        /// </summary>
        /// <typeparam name="T">Integration event type</typeparam>
        /// <typeparam name="TH">Integration event handler type</typeparam>
        public void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = GetEventKey<T>();
            var handlerType = typeof(TH);

            if (!HasSubscriptionsForEvent(eventName))
            {
                _eventHandlers.Add(eventName, new List<Subscription>());
                OnEventAdded?.Invoke(this, eventName);
            }

            if (_eventHandlers[eventName].Any(s => s.HandlerType == handlerType))
            {
                throw new ArgumentException(
                    $"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));
            }

            _eventHandlers[eventName].Add(Subscription.New(typeof(T), handlerType));

            OnEventAdded?.Invoke(this, eventName);
        }

        /// <summary>
        /// Removes a subscription for a given integration event and its handler.
        /// If the subscription is the last one for the event, the event will 
        /// be removed and OnEventRemoved will be raised so EventBus knows that
        /// the queue can be closed.
        /// </summary>
        /// <typeparam name="T">Integration event type to remove subscription</typeparam>
        /// <typeparam name="TH">Integration event type handler to remove subscription</typeparam>
        public void RemoveSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = GetEventKey<T>();
            var subscriptionToRemove = FindSubscriptionToRemove(eventName, typeof(TH));

            if (subscriptionToRemove != null)
            {
                _eventHandlers[eventName].Remove(subscriptionToRemove);

                if (!_eventHandlers[eventName].Any())
                {
                    RemoveEvent(eventName);
                }
            }
        }

        public bool HasSubscriptionsForEvent<T>()
            where T : IntegrationEvent
        {
            var eventName = GetEventKey<T>();

            return HasSubscriptionsForEvent(eventName);
        }

        public bool HasSubscriptionsForEvent(string eventName)
            => _eventHandlers.ContainsKey(eventName);

        public IEnumerable<Subscription> GetSubscriptionsForEvent(string eventName)
            => _eventHandlers[eventName];

        private void RemoveEvent(string eventName)
        {
            _eventHandlers.Remove(eventName);

            OnEventRemoved?.Invoke(this, eventName);
        }


        private Subscription FindSubscriptionToRemove(string eventName, Type eventHandlerType)
        {
            if (!HasSubscriptionsForEvent(eventName))
            {
                return null;
            }

            return _eventHandlers[eventName]
                .SingleOrDefault(s => s.HandlerType == eventHandlerType);
        }
    }
}
