using AutoMapper;
using BinaryDiff.Result.Domain.Enums;
using BinaryDiff.Result.Domain.Models;
using BinaryDiff.Result.Infrastructure.Repositories;
using BinaryDiff.Result.WebApi.Events.IntegrationEventHandlers;
using BinaryDiff.Result.WebApi.Events.IntegrationEvents;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace BinaryDiff.Result.UnitTests.WebApi
{
    public class NewResultIntegrationEventHandlerTests
    {
        /// <summary>
        /// Tests if handler is logging exception before throwing it
        /// </summary>
        [Fact]
        public void Handle_logs_exception_before_throwing_it()
        {
            // Arrange
            var newEvent = new NewResultIntegrationEvent();
            var anyException = new Exception($"Failed to process {nameof(NewResultIntegrationEvent)}: {newEvent.Id}");

            var logger = new Mock<ILogger<NewResultIntegrationEventHandler>>();
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(s => s.DiffResultsRepository)
                .Throws(anyException);

            var handler = GetHandler(logger, uow: uow);

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
        /// Tests if the diff result is being saved on repository 
        /// with the correct information
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Handle_saves_result_on_repository()
        {
            // Arrange
            var diffResult = new DiffResult
            {
                TriggerInputId = Guid.NewGuid().ToString(),
                TriggerInputPosition = InputPosition.Left,
                DiffId = Guid.NewGuid(),
                Result = ResultType.Different,
                OpposingInputId = Guid.NewGuid().ToString(),
                Differences = new InputDifference[] { new InputDifference(1, 2) }
            };

            var mockRepo = new Mock<IDiffResultsRepository>();

            var uow = new Mock<IUnitOfWork>();
            uow.Setup(s => s.DiffResultsRepository)
                .Returns(mockRepo.Object);

            var mapper = new Mock<IMapper>();
            mapper.Setup(s => s.Map<DiffResult>(It.IsAny<NewResultIntegrationEvent>()))
                .Returns(diffResult);

            var handler = GetHandler(mapper: mapper, uow: uow);

            // Act
            await handler.Handle(new NewResultIntegrationEvent());

            // Assert
            mockRepo.Verify(r =>
                r.Add(It.Is<DiffResult>(dr =>
                    dr.TriggerInputId == diffResult.TriggerInputId &&
                    dr.TriggerInputPosition == diffResult.TriggerInputPosition &&
                    dr.DiffId == diffResult.DiffId &&
                    dr.Result == diffResult.Result &&
                    dr.Differences.Count == diffResult.Differences.Count)
                ),
                Times.Once
            );
            uow.Verify(u =>
                u.SaveChangesAsync(),
                Times.Once
            );
        }

        /// <summary>
        /// Tests if handler will skip results that are already registered on repository, 
        /// to avoid duplication if the message was resent
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Handle_doesnt_save_result_if_already_exists()
        {
            // Arrange
            var newEvent = new NewResultIntegrationEvent { InputId = Guid.NewGuid().ToString() };

            var mapper = new Mock<IMapper>();
            mapper.Setup(s => s.Map<DiffResult>(It.IsAny<NewResultIntegrationEvent>()))
                .Returns(new DiffResult());

            var mockRepo = new Mock<IDiffResultsRepository>();
            mockRepo.Setup(s => s.Get(It.IsAny<Expression<Func<DiffResult, bool>>>()))
                .Returns(() =>
                    new DiffResult[] {
                        new DiffResult { TriggerInputId = newEvent.InputId }
                    }
                    .AsQueryable()
                );

            var uow = new Mock<IUnitOfWork>();
            uow.Setup(s => s.DiffResultsRepository)
                .Returns(mockRepo.Object);

            var handler = GetHandler(mapper: mapper, uow: uow);

            // Act
            await handler.Handle(newEvent);

            // Assert
            mockRepo.Verify(r =>
                r.Add(It.IsAny<DiffResult>()),
                Times.Never
            );
            uow.Verify(u =>
                u.SaveChangesAsync(),
                Times.Never
            );
        }

        private NewResultIntegrationEventHandler GetHandler(
            IMock<ILogger<NewResultIntegrationEventHandler>> logger = null,
            IMock<IMapper> mapper = null,
            IMock<IUnitOfWork> uow = null
        )
        {
            logger = logger ?? new Mock<ILogger<NewResultIntegrationEventHandler>>();
            mapper = mapper ?? new Mock<IMapper>();
            uow = uow ?? new Mock<IUnitOfWork>();

            return new NewResultIntegrationEventHandler(logger.Object, mapper.Object, uow.Object);
        }
    }
}
