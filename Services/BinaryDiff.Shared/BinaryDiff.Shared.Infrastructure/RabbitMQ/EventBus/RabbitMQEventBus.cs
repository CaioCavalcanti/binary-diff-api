using BinaryDiff.Shared.Infrastructure.RabbitMQ.Connections;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace BinaryDiff.Shared.Infrastructure.RabbitMQ.EventBus
{
    public class RabbitMQEventBus : IRabbitMQEventBus
    {
        private readonly ILogger _logger;
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly int _retryCount;
        private readonly string _exchangeName;
        private readonly RabbitMQExchangeType _exchangeType;

        public RabbitMQEventBus(
            IRabbitMQPersistentConnection persistentConnection,
            ILogger<RabbitMQEventBus> logger,
            string name,
            RabbitMQExchangeType type = RabbitMQExchangeType.Direct,
            int retryCount = 5
        )
        {
            _persistentConnection = persistentConnection;
            _retryCount = retryCount;
            _logger = logger;
            _exchangeName = name;
            _exchangeType = type;
        }

        public void Publish<TEvent>(TEvent @event)
            where TEvent : IntegrationEvent
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            using (var channel = _persistentConnection.CreateModel())
            {
                channel.ExchangeDeclare(
                    exchange: _exchangeName,
                    type: Enum.GetName(typeof(RabbitMQExchangeType), _exchangeType).ToLower());

                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);

                RabbitMQFactory.GetDefaultRetryPolicy(_logger, _retryCount)
                    .Execute(() =>
                    {
                        channel.BasicPublish(
                            exchange: _exchangeName,
                            routingKey: @event.GetType().Name,
                            body: body);
                    });
            }
        }
    }
}
