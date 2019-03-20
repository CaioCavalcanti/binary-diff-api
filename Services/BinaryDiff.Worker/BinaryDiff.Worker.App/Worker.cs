using BinaryDiff.Shared.Infrastructure.RabbitMQ.EventBus;
using BinaryDiff.Worker.App.Events.IntegrationEventHandlers;
using BinaryDiff.Worker.App.Events.IntegrationEvents;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace BinaryDiff.Worker.App
{
    public class Worker
    {
        private static AutoResetEvent _closing = new AutoResetEvent(false);

        private readonly ILogger _logger;
        private readonly IRabbitMQEventBus _eventBus;

        public Worker(ILogger<Worker> logger, IRabbitMQEventBus eventBus)
        {
            _logger = logger;
            _eventBus = eventBus;
        }

        public void ListenToNewInputs()
        {
            _logger.LogInformation($"Subscribing to event {nameof(NewInputIntegrationEvent)}");

            _eventBus.Subscribe<NewInputIntegrationEvent, NewInputIntegrationEventHandler>();

            _logger.LogInformation($"Subscription created, listening to event {nameof(NewInputIntegrationEvent)}");

            _logger.LogInformation("Press CTRL + C to close the app");

            Console.CancelKeyPress += (s, args) =>
            {
                _logger.LogInformation($"Subscribing to event {nameof(NewInputIntegrationEvent)}");

                _eventBus.Unsubscribe<NewInputIntegrationEvent, NewInputIntegrationEventHandler>();

                _logger.LogInformation("Closing worker app...");

                _closing.Set();
            };

            _closing.WaitOne();
        }
    }
}
