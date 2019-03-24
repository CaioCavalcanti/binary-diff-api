using BinaryDiff.Shared.Infrastructure.RabbitMQ.EventBus;
using BinaryDiff.Worker.App.Events.IntegrationEventHandlers;
using BinaryDiff.Worker.App.Events.IntegrationEvents;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;
using System;
using Xunit;

namespace BinaryDiff.Worker.UnitTests.App
{
    public class WorkerTests
    {
        /// <summary>
        /// Tests if worker subscribes to new input events on event bus
        /// </summary>
        [Fact]
        public void ListenToNewInputs_subscribes_to_NewInputIntegrationEvent()
        {
            // Arrange
            var logger = new Mock<ILogger<BinaryDiff.Worker.App.Worker>>();
            var eventBus = new Mock<IRabbitMQEventBus>();
            var worker = new BinaryDiff.Worker.App.Worker(logger.Object, eventBus.Object);

            // Act
            worker.ListenToNewInputs(TimeSpan.FromSeconds(.5));

            // Assert
            eventBus.Verify(eb =>
                eb.Subscribe<NewInputIntegrationEvent, NewInputIntegrationEventHandler>(),
                Times.Once
            );
        }

        /// <summary>
        /// Tests if worker logs exception before throwing it
        /// </summary>
        [Fact]
        public void ListenToNewInputs_logs_exception_before_throwing_on_exception()
        {
            // Arrange
            var anyException = new Exception("An error occurred when listening to new inputs!");

            var eventBus = new Mock<IRabbitMQEventBus>();
            eventBus
                .Setup(s => s.Subscribe<NewInputIntegrationEvent, NewInputIntegrationEventHandler>())
                .Throws(anyException);

            var logger = new Mock<ILogger<BinaryDiff.Worker.App.Worker>>();

            var worker = new BinaryDiff.Worker.App.Worker(logger.Object, eventBus.Object);

            // Act
            Assert.Throws<Exception>(() => worker.ListenToNewInputs(TimeSpan.FromSeconds(.5)));

            // Assert
            logger.Verify(l =>
                l.Log(
                    LogLevel.Error,
                    0,
                    It.Is<FormattedLogValues>(v => v.ToString() == anyException.Message),
                    It.Is<Exception>(e => e == anyException),
                    It.IsAny<Func<object, Exception, string>>())
            );
        }

        /// <summary>
        /// Tests if worker unsubscribe from event on event bus if cancel event was raised on app
        /// </summary>
        [Fact]
        public void ListenToNewInputs_unsubscribe_from_event_when_closing()
        {
            // Arrange
            var logger = new Mock<ILogger<BinaryDiff.Worker.App.Worker>>();
            var eventBus = new Mock<IRabbitMQEventBus>();
            var worker = new BinaryDiff.Worker.App.Worker(logger.Object, eventBus.Object);

            // Act
            worker.ListenToNewInputs(TimeSpan.FromSeconds(.5));

            // Assert
            eventBus.Verify(eb =>
                eb.Unsubscribe<NewInputIntegrationEvent, NewInputIntegrationEventHandler>(),
                Times.Once
            );
        }
    }
}
