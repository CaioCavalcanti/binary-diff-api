using BinaryDiff.Worker.App.Events.IntegrationEventHandlers;
using BinaryDiff.Worker.App.Events.IntegrationEvents;
using BinaryDiff.Worker.Infrastructure.EventBus;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace BinaryDiff.Worker.App
{
    public class Worker
    {
        private static AutoResetEvent _closing = new AutoResetEvent(false);

        private readonly ILogger _logger;
        private readonly IInputEventBus _inputEventBus;

        public Worker(ILogger<Worker> logger, IInputEventBus inputEventBus)
        {
            _logger = logger;
            _inputEventBus = inputEventBus;
        }

        public void ListenToNewInputs()
        {
            _logger.LogInformation($"Subscribing to event {nameof(NewInputIntegrationEvent)}");

            _inputEventBus.Subscribe<NewInputIntegrationEvent, NewInputIntegrationEventHandler>();

            _logger.LogInformation($"Subscription created, listening to event {nameof(NewInputIntegrationEvent)}");

            _logger.LogInformation("Press CTRL + C to close the app");

            Console.CancelKeyPress += (s, args) =>
            {
                _logger.LogInformation($"Subscribing to event {nameof(NewInputIntegrationEvent)}");

                _inputEventBus.Unsubscribe<NewInputIntegrationEvent, NewInputIntegrationEventHandler>();

                _logger.LogInformation("Closing worker app...");

                _closing.Set();
            };

            _closing.WaitOne();
        }
    }
}
