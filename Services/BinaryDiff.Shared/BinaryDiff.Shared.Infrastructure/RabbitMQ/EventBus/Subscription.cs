using Autofac;
using BinaryDiff.Shared.Infrastructure.RabbitMQ.Events;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BinaryDiff.Shared.Infrastructure.RabbitMQ.EventBus
{
    /// <summary>
    /// Subscription info for a given integration event and its handler
    /// </summary>
    public class Subscription
    {
        public Type HandlerType { get; }
        public Type EventType { get; }

        private Subscription(Type handlerType, Type eventType = null)
        {
            HandlerType = handlerType;
            EventType = eventType;
        }

        /// <summary>
        /// Resolves the integration event handler from scope and invokes (async)
        /// with a message received.
        /// </summary>
        /// <param name="message">Message received</param>
        /// <param name="scope">Lifetime scope</param>
        /// <returns></returns>
        public Task HandleAsync(string message, ILifetimeScope scope)
        {
            var eventData = JsonConvert.DeserializeObject(message, EventType);
            var handler = scope.ResolveOptional(HandlerType);
            var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(EventType);

            return (Task)concreteType.GetMethod("Handle")
                .Invoke(handler, new[] { eventData });
        }

        /// <summary>
        /// Creates a new subscription instance for a given integration event and its handler
        /// </summary>
        /// <param name="eventType">Instance of IntegrationEvent</param>
        /// <param name="handlerType">Instance of IntegrationEventHandler<IntegrationEvent></param>
        /// <returns>Subscription instance</returns>
        public static Subscription New(Type eventType, Type handlerType)
            => new Subscription(handlerType, eventType);
    }
}
