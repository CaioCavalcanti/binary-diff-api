using Autofac;
using BinaryDiff.Shared.Infrastructure.RabbitMQ;
using BinaryDiff.Shared.Infrastructure.RabbitMQ.Connections;
using BinaryDiff.Shared.Infrastructure.RabbitMQ.EventBus;
using Microsoft.Extensions.Logging;

namespace BinaryDiff.Worker.Infrastructure.EventBus
{
    public class InputEventBus : RabbitMQEventBus, IInputEventBus
    {
        public InputEventBus(
            IRabbitMQPersistentConnection connection,
            ILogger<InputEventBus> logger,
            ILifetimeScope autofac
        ) : base(connection, logger, autofac, RabbitMQExchangeType.Direct)
        {
        }
    }
}
