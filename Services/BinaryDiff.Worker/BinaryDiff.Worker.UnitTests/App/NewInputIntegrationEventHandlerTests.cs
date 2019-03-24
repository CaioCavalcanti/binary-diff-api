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
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BinaryDiff.Worker.UnitTests.App
{
    public class NewInputIntegrationEventHandlerTests
    {
        /// <summary>
        /// Tests if exception is being logged before being raised
        /// </summary>
        [Fact]
        public void Handle_logs_exception_before_throwing_it()
        {
            // Arrange
            var newEvent = new NewInputIntegrationEvent();
            var anyException = new Exception($"Failed to process event {newEvent.Id}");

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
                    It.Is<FormattedLogValues>(v => v.ToString() == anyException.Message),
                    It.Is<Exception>(e => e == anyException),
                    It.IsAny<Func<object, Exception, string>>())
            );
        }

        /// <summary>
        /// Tests if event handler tries to get the result for a input that was not found
        /// </summary>
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

        /// <summary>
        /// Tests if event handle considers input position larger if opposing position is null
        /// </summary>
        /// <param name="position">Position for the input received</param>
        /// <param name="expectedResultType">Expected diff result</param>
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
                logic: logic.Object
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

        /// <summary>
        /// Tests if event handler publishes a new result event on event bus
        /// </summary>
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
                logic: logic.Object
            );

            // Act
            handler.Handle(newInputEvent);

            // Assert
            eventBus.Verify(
                eb => eb.Publish(It.IsAny<NewResultIntegrationEvent>()),
                Times.Once
            );
        }

        [Theory]
        [MemberData(nameof(ExpectEqual))]
        [MemberData(nameof(ExpectLeftIsLarger))]
        [MemberData(nameof(ExpectRightIsLarger))]
        [MemberData(nameof(ExpectDifferentWithDifferences))]
        public void Handle_publishes_expected_result_on_event(
            InputData input,
            InputData opposingInput,
            DiffResult expectedResult
        )
        {
            // Arrange
            var resultEvent = new NewResultIntegrationEvent(input, string.Empty, expectedResult);
            var newInputEvent = new NewInputIntegrationEvent() { Position = input.Position };

            var eventBus = new Mock<IRabbitMQEventBus>();

            var repository = new Mock<IInputRepository>();
            repository
                .Setup(s => s.FindAsync(It.IsAny<string>()))
                .ReturnsAsync(() => input);

            repository
                .Setup(s => s.GetLastInputBeforeAsync(It.IsAny<Guid>(), It.IsAny<InputPosition>(), It.IsAny<DateTime>()))
                .ReturnsAsync(() => opposingInput);

            var handler = GetHandler(
                eventBus: eventBus,
                repository: repository,
                logic: new DiffLogic()
            );

            // Act
            handler.Handle(newInputEvent);

            // Assert
            eventBus.Verify(
                eb => eb.Publish(
                    It.Is<NewResultIntegrationEvent>(e =>
                        e.Result == expectedResult.Result &&
                        e.InputPosition == input.Position)
                )
            );
        }

        [Theory]
        [MemberData(nameof(ExpectDifferentWithDifferences))]
        public void Handle_should_return_expected_differences_for_different_inputs(
            InputData input,
            InputData opposingInput,
            DiffResult expectedResult
        )
        {
            // Arrange
            var resultEvent = new NewResultIntegrationEvent(input, string.Empty, expectedResult);
            var newInputEvent = new NewInputIntegrationEvent() { Position = input.Position };

            var eventBus = new Mock<IRabbitMQEventBus>();

            var repository = new Mock<IInputRepository>();
            repository
                .Setup(s => s.FindAsync(It.IsAny<string>()))
                .ReturnsAsync(() => input);

            repository
                .Setup(s => s.GetLastInputBeforeAsync(It.IsAny<Guid>(), It.IsAny<InputPosition>(), It.IsAny<DateTime>()))
                .ReturnsAsync(() => opposingInput);

            var handler = GetHandler(
                eventBus: eventBus,
                repository: repository,
                logic: new DiffLogic()
            );

            // Act
            handler.Handle(newInputEvent);

            // Assert
            eventBus.Verify(
                eb => eb.Publish(
                    It.Is<NewResultIntegrationEvent>(e =>
                        e.Result == ResultType.Different &&
                        e.Differences.Count == expectedResult.Differences.Count &&
                        e.Differences.All(_ => expectedResult.Differences.Contains(_))
                    )
                )
            );
        }

        public static NewInputIntegrationEventHandler GetHandler(
            IMock<ILogger<NewInputIntegrationEventHandler>> logger = null,
            IMock<IRabbitMQEventBus> eventBus = null,
            IMock<IInputRepository> repository = null,
            IDiffLogic logic = null
        )
        {
            var loggerMock = logger ?? new Mock<ILogger<NewInputIntegrationEventHandler>>();
            var eventBusMock = eventBus ?? new Mock<IRabbitMQEventBus>();
            var repositoryMock = repository ?? new Mock<IInputRepository>();
            logic = logic ?? new Mock<IDiffLogic>().Object;

            return new NewInputIntegrationEventHandler(
                loggerMock.Object,
                eventBusMock.Object,
                repositoryMock.Object,
                logic
            );
        }

        /// <summary>
        /// Resolve test data for scenarios where both inputs are equal
        /// </summary>
        public static IEnumerable<object[]> ExpectEqual => new List<object[]>
        {
            NewExpectedResult(null, null, ResultType.Equal),
            NewExpectedResult("", "", ResultType.Equal),
            NewExpectedResult("abc", "abc", ResultType.Equal)
        };

        /// <summary>
        /// Resolve test data for different scenarios where left is larger
        /// </summary>
        public static IEnumerable<object[]> ExpectLeftIsLarger => new List<object[]>
        {
            NewExpectedResult("abc", null, ResultType.LeftIsLarger),
            NewExpectedResult("abc", "", ResultType.LeftIsLarger),
            NewExpectedResult("abc", "a", ResultType.LeftIsLarger)
        };

        /// <summary>
        /// Resolve test data for different scenarios with expected results
        /// </summary>
        public static IEnumerable<object[]> ExpectRightIsLarger => new List<object[]>
        {
            NewExpectedResult("abc", null, ResultType.RightIsLarger, inputPosition: InputPosition.Right, opposingPosition: InputPosition.Left),
            NewExpectedResult("abc", "", ResultType.RightIsLarger, inputPosition: InputPosition.Right, opposingPosition: InputPosition.Left),
            NewExpectedResult("abc", "a", ResultType.RightIsLarger, inputPosition: InputPosition.Right, opposingPosition: InputPosition.Left),
        };

        /// <summary>
        /// Resolve test data for different scenarios with expected results
        /// </summary>
        public static IEnumerable<object[]> ExpectDifferentWithDifferences => new List<object[]>
        {
            NewExpectedResult("abc", "xbc", ResultType.Different, new Dictionary<int, int> { { 0, 1 } }),
            NewExpectedResult("abc", "axc", ResultType.Different, new Dictionary<int, int> { { 1, 1 } }),
            NewExpectedResult("abc", "xyz", ResultType.Different, new Dictionary<int, int> { { 0, 3 } }),
            NewExpectedResult("abc", "axx", ResultType.Different, new Dictionary<int, int> { { 1, 2 } }),
            NewExpectedResult("abc", "abx", ResultType.Different, new Dictionary<int, int> { { 2, 1 } }),
            NewExpectedResult("abc", "xbx", ResultType.Different, new Dictionary<int, int> { { 0, 1 }, { 2, 1 } }),
            NewExpectedResult("abc", "xxc", ResultType.Different, new Dictionary<int, int> { { 0, 2 } })
        };

        private static object[] NewExpectedResult(
            string inputData,
            string opposingData,
            ResultType expectedResult,
            IDictionary<int, int> expectedDifferences = null,
            InputPosition inputPosition = InputPosition.Left,
            InputPosition opposingPosition = InputPosition.Right
        )
             => new object[] {
                new InputData(inputPosition, inputData),
                new InputData(opposingPosition, opposingData),
                new DiffResult(expectedResult, expectedDifferences)
            };
    }
}
