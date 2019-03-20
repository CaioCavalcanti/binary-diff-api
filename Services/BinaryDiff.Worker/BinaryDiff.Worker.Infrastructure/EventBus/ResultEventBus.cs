using Autofac;
using BinaryDiff.Shared.Infrastructure.RabbitMQ;
using BinaryDiff.Shared.Infrastructure.RabbitMQ.Connections;
using BinaryDiff.Shared.Infrastructure.RabbitMQ.EventBus;
using Microsoft.Extensions.Logging;

namespace BinaryDiff.Worker.Infrastructure.EventBus
{
    public class ResultEventBus : RabbitMQEventBus, IResultEventBus
    {
        public ResultEventBus(
            IRabbitMQPersistentConnection connection,
            ILogger<RabbitMQEventBus> logger,
            ILifetimeScope autofac
        ) : base(connection, logger, autofac, nameof(ResultEventBus), RabbitMQExchangeType.Direct)
        {
        }
    }
}
