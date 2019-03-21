using BinaryDiff.Shared.Infrastructure.RabbitMQ.EventBus;
using BinaryDiff.Worker.App.Events.IntegrationEventHandlers;
using BinaryDiff.Worker.App.Events.IntegrationEvents;
using BinaryDiff.Worker.Domain.Enums;
using BinaryDiff.Worker.Domain.Logic;
using BinaryDiff.Worker.Domain.Models;
using BinaryDiff.Worker.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;
using System;
using Xunit;

namespace BinaryDiff.Worker.UnitTests.App
{
    public class NewInputIntegrationEventHandlerTests
    {
        [Fact]
        public void Handle_logs_exception_before_throwing_it()
        {
            // Arrange
            var anyException = new Exception("Something went wrong....");
            var newEvent = new NewInputIntegrationEvent();
            var expectedLogMessage = $"Failed to process event {newEvent.Id}";

            var failingRepo = new Mock<IInputRepository>();
            failingRepo
                .Setup(s => s.FindAsync(It.IsAny<string>()))
                .Throws(anyException);

            var logger = new Mock<ILogger<NewInputIntegrationEventHandler>>();

            var handler = GetHandler(repository: failingRepo, logger: logger);

            // Act
            Assert.ThrowsAsync<Exception>(() => handler.Handle(newEvent));

            // Assert
            logger.Verify(l =>
                l.Log(
                    LogLevel.Error,
                    0,
                    It.Is<FormattedLogValues>(v => v.ToString() == expectedLogMessage),
                    It.Is<Exception>(e => e == anyException),
                    It.IsAny<Func<object, Exception, string>>())
            );
        }

        [Fact]
        public void Handle_doesnt_compare_data_if_input_not_found()
        {
            // Arrange
            var newInputEvent = new NewInputIntegrationEvent();

            var eventBus = new Mock<IRabbitMQEventBus>();
            var logic = new Mock<IDiffLogic>();

            var repository = new Mock<IInputRepository>();
            repository
                .Setup(s => s.FindAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            var handler = GetHandler(eventBus: eventBus, repository: repository);

            // Act
            handler.Handle(newInputEvent);

            // Assert
            logic.Verify(
                df => df.CompareData(It.IsAny<InputData>(), It.IsAny<InputData>()),
                Times.Never
            );

            eventBus.Verify(
                eb => eb.Publish(It.IsAny<NewResultIntegrationEvent>()),
                Times.Never
            );
        }

        [Theory]
        [InlineData(InputPosition.Left, ResultType.LeftIsLarger)]
        [InlineData(InputPosition.Right, ResultType.RightIsLarger)]
        public void Handle_consider_input_larger_if_no_opposing_input_found(InputPosition position, ResultType expectedResultType)
        {
            // Arrange
            var input = new InputData(position, "test");
            var expectedResult = new DiffResult(expectedResultType, null);
            var resultEvent = new NewResultIntegrationEvent(input, string.Empty, expectedResult);
            var newInputEvent = new NewInputIntegrationEvent() { Position = input.Position };

            var eventBus = new Mock<IRabbitMQEventBus>();

            var logic = new Mock<IDiffLogic>();
            logic.Setup(s => s.CompareData(It.IsAny<InputData>(), It.IsAny<InputData>()))
                .Returns(expectedResult);

            var repository = new Mock<IInputRepository>();
            repository
                .Setup(s => s.FindAsync(It.IsAny<string>()))
                .ReturnsAsync(() => input);

            repository
                .Setup(s => s.GetLastInputBeforeAsync(It.IsAny<Guid>(), It.IsAny<InputPosition>(), It.IsAny<DateTime>()))
                .ReturnsAsync(() => null);

            var handler = GetHandler(
                eventBus: eventBus,
                repository: repository,
                logic: logic
            );

            // Act
            handler.Handle(newInputEvent);

            // Assert
            eventBus.Verify(
                eb => eb.Publish(
                    It.Is<NewResultIntegrationEvent>(e => e.Result == expectedResult.Result)
                )
            );
        }

        [Fact]
        public void Handle_publishes_new_result_event()
        {
            // Arrange
            var newInputEvent = new NewInputIntegrationEvent();
            var anyResult = new DiffResult(ResultType.Different, null);

            var eventBus = new Mock<IRabbitMQEventBus>();

            var logic = new Mock<IDiffLogic>();
            logic.Setup(s => s.CompareData(It.IsAny<InputData>(), It.IsAny<InputData>()))
                .Returns(anyResult);

            var repository = new Mock<IInputRepository>();
            repository
                .Setup(s => s.FindAsync(It.IsAny<string>()))
                .ReturnsAsync(() => Mock.Of<InputData>());

            var handler = GetHandler(
                eventBus: eventBus,
                repository: repository,
                logic: logic
            );

            // Act
            handler.Handle(newInputEvent);

            // Assert
            eventBus.Verify(
                eb => eb.Publish(It.IsAny<NewResultIntegrationEvent>()),
                Times.Once
            );
        }

        public static NewInputIntegrationEventHandler GetHandler(
            IMock<ILogger<NewInputIntegrationEventHandler>> logger = null,
            IMock<IRabbitMQEventBus> eventBus = null,
            IMock<IInputRepository> repository = null,
            IMock<IDiffLogic> logic = null
        )
        {
            var loggerMock = logger ?? new Mock<ILogger<NewInputIntegrationEventHandler>>();
            var eventBusMock = eventBus ?? new Mock<IRabbitMQEventBus>();
            var repositoryMock = repository ?? new Mock<IInputRepository>();
            var logicMock = logic ?? new Mock<IDiffLogic>();

            return new NewInputIntegrationEventHandler(
                loggerMock.Object,
                eventBusMock.Object,
                repositoryMock.Object,
                logicMock.Object
            );
        }
    }
}
