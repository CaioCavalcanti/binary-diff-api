using RabbitMQ.Client;

namespace BinaryDiff.Shared.Infrastructure.RabbitMQ.EventBus
{
    public interface IRabbitMQEventBus
    {
        void Publish<TEvent>(TEvent @event)
            where TEvent : IntegrationEvent;

        IModel CreateChannel(string queueName);
    }
}
