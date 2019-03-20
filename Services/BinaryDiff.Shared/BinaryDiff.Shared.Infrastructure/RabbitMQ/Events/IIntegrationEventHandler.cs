using System.Threading.Tasks;

namespace BinaryDiff.Shared.Infrastructure.RabbitMQ.Events
{
    public interface IIntegrationEventHandler<TIntegrationEvent>
        where TIntegrationEvent : IntegrationEvent
    {
        Task Handle(TIntegrationEvent @event);
    }
}
