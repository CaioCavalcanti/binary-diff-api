using BinaryDiff.Shared.Infrastructure.RabbitMQ;
using BinaryDiff.Shared.Infrastructure.RabbitMQ.Connections;
using BinaryDiff.Shared.Infrastructure.RabbitMQ.EventBus;
using Microsoft.Extensions.Logging;

namespace BinaryDiff.Input.Infrastructure.EventBus
{
    public class InputEventBus : RabbitMQEventBus, IInputEventBus
    {
        public InputEventBus(IRabbitMQPersistentConnection persistentConnection, ILogger<InputEventBus> logger)
            : base(persistentConnection, logger, nameof(InputEventBus), RabbitMQExchangeType.Direct)
        {
        }
    }
}
