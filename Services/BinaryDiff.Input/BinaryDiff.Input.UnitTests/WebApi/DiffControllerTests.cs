using AutoMapper;
using BinaryDiff.Input.Domain.Enums;
using BinaryDiff.Input.Domain.Models;
using BinaryDiff.Input.Infrastructure.Repositories;
using BinaryDiff.Input.WebApi.Controllers;
using BinaryDiff.Input.WebApi.IntegrationEvents;
using BinaryDiff.Input.WebApi.Mappers;
using BinaryDiff.Input.WebApi.ViewModels;
using BinaryDiff.Shared.Infrastructure.RabbitMQ.EventBus;
using BinaryDiff.Shared.WebApi.ResultMessages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace BinaryDiff.Input.UnitTests.WebApi
{
    public class DiffControllerTests
    {
        private readonly IMapper _mapper;
        private readonly ILogger<DiffController> _logger = Mock.Of<ILogger<DiffController>>();

        public DiffControllerTests()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DiffMapper>();
                cfg.AddProfile<InputMapper>();
            });

            _mapper = mapperConfig.CreateMapper();
        }

        /// <summary>
        /// Tests if PostAsync creates new diff and returns its id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostAsync_creates_new_diff()
        {
            // Arrange
            var inputRepository = new Mock<IInputRepository>();
            var diffRepository = new Mock<IDiffRepository>();
            var eventBus = new Mock<IRabbitMQEventBus>();

            var controller = new DiffController(
                _logger,
                diffRepository.Object,
                inputRepository.Object,
                _mapper,
                eventBus.Object);

            // Act
            var response = await controller.PostAsync();

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(response);
            var result = Assert.IsType<DiffViewModel>(createdResult.Value);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);

            diffRepository.Verify(dr =>
                dr.AddOneAsync(It.IsAny<Diff>()),
                Times.Once
            );
        }

        /// <summary>
        /// Tests if PostLeftAsync adds provided data into left position for a given diff id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostLeftAsync_adds_data_to_left_position_of_given_diff()
        {
            // Arrange
            var diffId = Guid.NewGuid();
            var newInput = new NewInputViewModel { Data = "foobar" };
            var inputRepository = new Mock<IInputRepository>();
            var controller = SetupDiffControllerForPostInput(diffId, inputRepository);

            // Act
            var response = await controller.PostLeftAsync(diffId, newInput);

            // Assert
            AssertInputCreatedOnPosition(response, diffId, newInput.Data, InputPosition.Left, inputRepository);
        }

        /// <summary>
        /// Tests if PostLeftAsync publishes new input event on event bus
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostLeftAsync_publishes_NewInputIntegrationEvent_on_event_bus()
        {
            // Arrange
            var diffId = Guid.NewGuid();
            var newInput = new NewInputViewModel { Data = "foobar" };
            var eventBus = new Mock<IRabbitMQEventBus>();
            var controller = SetupDiffControllerForPostInput(diffId, eventBus: eventBus);

            // Act
            var response = await controller.PostLeftAsync(diffId, newInput);

            // Assert
            AssertNewInputIntegrationEventPublished(response, diffId, InputPosition.Left, eventBus);
        }

        /// <summary>
        /// Tests if PostLeftAsync returns resource not found for a diff id that doesn't exist
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostLeftAsync_returns_ResourceNotFoundForIdResultMessage_if_diff_id_not_found()
        {
            // Arrange
            var idNotFound = Guid.NewGuid();
            var controller = SetupDiffControllerForPostInput(idNotFound, expectDiffToExist: false);

            // Act
            var response = await controller.PostLeftAsync(idNotFound, new NewInputViewModel());

            // Assert
            AssertDiffIdNotFound(response, idNotFound);
        }

        /// <summary>
        /// Tests if PostRightAsync adds provided data into right position for a given diff id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostRightAsync_adds_data_to_right_position_of_given_diff()
        {
            // Arrange
            var diffId = Guid.NewGuid();
            var newInput = new NewInputViewModel { Data = "foobar" };

            var inputRepository = new Mock<IInputRepository>();
            var controller = SetupDiffControllerForPostInput(diffId, inputRepository);

            // Act
            var response = await controller.PostRightAsync(diffId, newInput);

            // Assert
            AssertInputCreatedOnPosition(response, diffId, newInput.Data, InputPosition.Right, inputRepository);
        }

        /// <summary>
        /// Tests if PostRightAsync publishes new input event on event bus
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostRightAsync_publishes_NewInputIntegrationEvent_on_event_bus()
        {
            // Arrange
            var diffId = Guid.NewGuid();
            var newInput = new NewInputViewModel { Data = "foobar" };
            var eventBus = new Mock<IRabbitMQEventBus>();
            var controller = SetupDiffControllerForPostInput(diffId, eventBus: eventBus);

            // Act
            var response = await controller.PostRightAsync(diffId, newInput);

            // Assert
            AssertNewInputIntegrationEventPublished(response, diffId, InputPosition.Right, eventBus);
        }

        /// <summary>
        /// Tests if PostRightAsync returns resource not found for a diff id that doesn't exist
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostRightAsync_returns_ResourceNotFoundForIdResultMessage_if_diff_id_not_found()
        {
            // Arrange
            var idNotFound = Guid.NewGuid();
            var controller = SetupDiffControllerForPostInput(idNotFound, expectDiffToExist: false);

            // Act
            var response = await controller.PostRightAsync(idNotFound, new NewInputViewModel());

            // Assert
            AssertDiffIdNotFound(response, idNotFound);
        }

        private DiffController SetupDiffControllerForPostInput(
            Guid expectedDiffId,
            Mock<IInputRepository> inputRepository = null,
            Mock<IRabbitMQEventBus> eventBus = null,
            bool expectDiffToExist = true
        )
        {
            eventBus = eventBus ?? new Mock<IRabbitMQEventBus>();

            inputRepository = inputRepository ?? new Mock<IInputRepository>();
            inputRepository.Setup(s => s.AddOneAsync(It.IsAny<InputData>()))
                .Returns<InputData>(model =>
                {
                    model.Id = Guid.NewGuid().ToString();
                    return Task.FromResult(model);
                });

            var diffRepository = new Mock<IDiffRepository>();
            diffRepository
                .Setup(s => s.FindAsync(It.IsAny<Expression<Func<Diff, bool>>>()))
                .ReturnsAsync(() =>
                {
                    if (expectDiffToExist)
                    {
                        return new List<Diff>()
                        {
                            new Diff { UUID = expectedDiffId }
                        };
                    }
                    return null;
                });

            return new DiffController(
                _logger,
                diffRepository.Object,
                inputRepository.Object,
                _mapper,
                eventBus.Object
            );
        }

        private void AssertInputCreatedOnPosition(
            IActionResult response,
            Guid expectedDiffId,
            string data,
            InputPosition expectedPosition,
            Mock<IInputRepository> inputRepository
        )
        {
            // AssertAssert
            var createdResult = Assert.IsType<CreatedResult>(response);
            var result = Assert.IsType<InputViewModel>(createdResult.Value);

            Assert.NotNull(result);
            Assert.Equal(expectedDiffId, result.DiffId);
            Assert.Equal(Enum.GetName(typeof(InputPosition), expectedPosition), result.Position);

            inputRepository.Verify(v =>
                v.AddOneAsync(
                    It.Is<InputData>(i =>
                        i.DiffId == expectedDiffId &&
                        i.Data == data
                    )
                )
            );
        }

        private void AssertNewInputIntegrationEventPublished(
            IActionResult response,
            Guid expectedDiffId,
            InputPosition expectedPosition,
            Mock<IRabbitMQEventBus> eventBus
        )
        {
            var createdResult = Assert.IsType<CreatedResult>(response);
            var result = Assert.IsType<InputViewModel>(createdResult.Value);

            eventBus.Verify(eb =>
                eb.Publish(It.Is<NewInputIntegrationEvent>(ie =>
                    ie.DiffId == expectedDiffId.ToString() &&
                    ie.Position == expectedPosition &&
                    ie.InputId == result.Id &&
                    ie.Timestamp == result.Timestamp
                ))
            );
        }

        private void AssertDiffIdNotFound(IActionResult response, Guid idNotFound)
        {
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(response);
            var result = Assert.IsType<ResourceNotFoundForIdResultMessage<Diff>>(notFoundResult.Value);
            Assert.Equal($"{nameof(Diff)} with ID '{idNotFound}' not found", result.Message);
        }
    }
}
