using BinaryDiff.Shared.Infrastructure.RabbitMQ.EventBus;
using BinaryDiff.Worker.App.Events.IntegrationEventHandlers;
using BinaryDiff.Worker.App.Events.IntegrationEvents;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinaryDiff.Worker.App
{
    /// <summary>
    /// Actual worker that subscribe to new inputs on event bus and publishes the results
    /// </summary>
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

        /// <summary>
        /// Subscribes to new input events and wait for cancel key (CTRL + C) or specified time span, if any
        /// </summary>
        /// <param name="time">TimeSpan to let the app run before closign it</param>
        public void ListenToNewInputs(TimeSpan? time = null)
        {
            try
            {
                _logger.LogInformation($"Subscribing to event {nameof(NewInputIntegrationEvent)}");

                _eventBus.Subscribe<NewInputIntegrationEvent, NewInputIntegrationEventHandler>();

                _logger.LogInformation($"Subscription created, listening to event {nameof(NewInputIntegrationEvent)}");

                Console.CancelKeyPress += OnCancel;

                SetWait(time);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred when listening to new inputs!");
                throw;
            }
        }

        /// <summary>
        /// Sets app to wait cancel key (CTRL + C) or a fixed time span (helps unit tests)
        /// </summary>
        /// <param name="time"></param>
        private void SetWait(TimeSpan? time)
        {
            _logger.LogInformation("Press CTRL + C to close the app");

            if (time.HasValue)
            {
                Task.Delay(time.Value);
                OnCancel(this, null);
            }
            else
            {
                _closing.WaitOne();
            }
        }

        /// <summary>
        /// Unsubscribe from event on event bus when cancelling the app execution
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnCancel(object sender, EventArgs args)
        {
            _logger.LogInformation($"Subscribing to event {nameof(NewInputIntegrationEvent)}");

            _eventBus.Unsubscribe<NewInputIntegrationEvent, NewInputIntegrationEventHandler>();

            _logger.LogInformation("Closing worker app...");

            _closing.Set();
        }
    }
}
