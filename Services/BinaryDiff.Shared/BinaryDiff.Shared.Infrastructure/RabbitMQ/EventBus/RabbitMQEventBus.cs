using Autofac;
using BinaryDiff.Shared.Infrastructure.RabbitMQ.Connections;
using BinaryDiff.Shared.Infrastructure.RabbitMQ.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BinaryDiff.Shared.Infrastructure.RabbitMQ.EventBus
{
    public class RabbitMQEventBus : IRabbitMQEventBus
    {
        const string EXCHANGE_NAME = "BinaryDiff.EventBus";

        private readonly ILifetimeScope _autofac;
        private readonly ILogger _logger;
        private readonly IRabbitMQPersistentConnection _connection;
        private readonly int _retryCount;
        private readonly RabbitMQExchangeType _exchangeType;
        private readonly ISubscriptionManager _subscriptionManager;

        private string _queueName;
        private IModel _consumerChannel;

        public RabbitMQEventBus(
            IRabbitMQPersistentConnection persistentConnection,
            ILogger<RabbitMQEventBus> logger,
            ILifetimeScope autofac,
            RabbitMQExchangeType type = RabbitMQExchangeType.Direct,
            int retryCount = 5
        )
        {
            _connection = persistentConnection;
            _logger = logger;
            _autofac = autofac;
            _exchangeType = type;
            _retryCount = retryCount;

            _subscriptionManager = new SubscriptionManager();
            _subscriptionManager.OnEventRemoved += OnSubscriptionManagerEventRemoved;
            _subscriptionManager.OnEventAdded += OnSubscriptionManagerEventAdded;

            _consumerChannel = CreateConsumerChannel();
        }

        private IModel CreateConsumerChannel()
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            var channel = _connection.CreateModel();

            channel.ExchangeDeclare(
                exchange: EXCHANGE_NAME,
                type: Enum.GetName(typeof(RabbitMQExchangeType), _exchangeType).ToLower());

            _queueName = channel.QueueDeclare().QueueName;

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var eventName = ea.RoutingKey;
                var message = Encoding.UTF8.GetString(ea.Body);

                HandleEvent(eventName, message);
            };

            channel.BasicConsume(queue: _queueName,
                autoAck: false,
                consumer: consumer);

            channel.CallbackException += (sender, ea) =>
            {
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
            };

            return channel;
        }

        /// <summary>
        /// Get all subscriptions for a given event and invoke
        /// its handlers to process the received message in parallel
        /// </summary>
        /// <param name="eventName">Integration event name</param>
        /// <param name="message">Message to process on event handler</param>
        private void HandleEvent(string eventName, string message)
        {
            try
            {
                if (!_subscriptionManager.HasSubscriptionsForEvent(eventName))
                {
                    return;
                }

                using (var scope = _autofac.BeginLifetimeScope(nameof(RabbitMQEventBus)))
                {
                    var subscriptions = _subscriptionManager.GetSubscriptionsForEvent(eventName);
                    var handleTasks = new List<Task>();

                    foreach (var subscription in subscriptions)
                    {
                        handleTasks.Add(subscription.HandleAsync(message, scope));
                    }

                    Task.WaitAll(handleTasks.ToArray());
                }
            } catch(Exception ex)
            {
                // TODO: handle ex
                _logger.LogError(ex, $"An error occurred processing message for event {eventName}");
            }
        }

        /// <summary>
        /// Publishes an integration event using RabbitMQ basic publish
        /// Uses default retry policy configured on app
        /// </summary>
        /// <typeparam name="TEvent">IntegrationEvent</typeparam>
        /// <param name="event">Integration event to be published</param>
        public void Publish<TEvent>(TEvent @event)
            where TEvent : IntegrationEvent
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            using (var channel = _connection.CreateModel())
            {
                channel.ExchangeDeclare(
                    exchange: EXCHANGE_NAME,
                    type: Enum.GetName(typeof(RabbitMQExchangeType), _exchangeType).ToLower());

                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);

                RabbitMQFactory.GetDefaultRetryPolicy(_logger, _retryCount)
                    .Execute((Action)(() =>
                    {
                        channel.BasicPublish(
                            exchange: EXCHANGE_NAME,
                            routingKey: @event.GetType().Name,
                            body: body);
                    }));
            }
        }

        /// <summary>
        /// Bind queue when a new event is added to subscription manager
        /// </summary>
        /// <param name="sender">Instance of SubscriptionManager</param>
        /// <param name="eventName">Name of event type that was added</param>
        private void OnSubscriptionManagerEventAdded(object sender, string eventName)
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            using (var channel = _connection.CreateModel())
            {
                channel.QueueBind(
                    queue: _queueName,
                    exchange: EXCHANGE_NAME,
                    routingKey: eventName
                );
            }
        }

        /// <summary>
        /// Unbind queu when an event is removed from subscription manager
        /// </summary>
        /// <param name="sender">Instance of SubscriptionManager</param>
        /// <param name="eventName">Name of event type that was removed</param>
        private void OnSubscriptionManagerEventRemoved(object sender, string eventName)
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            using (var channel = _connection.CreateModel())
            {
                channel.QueueUnbind(
                    queue: _queueName,
                    exchange: EXCHANGE_NAME,
                    routingKey: eventName
                );

                if (!_subscriptionManager.IsEmpty) return;

                _queueName = string.Empty;
                _consumerChannel.Close();
            }
        }

        /// <summary>
        /// Closes consumer channel on dispose
        /// </summary>
        public void Dispose()
        {
            _consumerChannel?.Close();
        }

        /// <summary>
        /// Subscribe to a 
        /// </summary>
        /// <typeparam name="T">Integration event of type</typeparam>
        /// <typeparam name="TH">Integration event handler type</typeparam>
        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            _subscriptionManager.AddSubscription<T, TH>();
        }

        /// <summary>
        /// Unsubscribe event from event bus
        /// </summary>
        /// <typeparam name="T">Integration event</typeparam>
        /// <typeparam name="TH">Integration event handler</typeparam>
        public void Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            _subscriptionManager.RemoveSubscription<T, TH>();
        }
    }
}