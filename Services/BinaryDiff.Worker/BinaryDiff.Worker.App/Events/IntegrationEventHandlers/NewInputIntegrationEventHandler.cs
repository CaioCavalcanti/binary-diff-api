using BinaryDiff.Shared.Infrastructure.RabbitMQ.EventBus;
using BinaryDiff.Shared.Infrastructure.RabbitMQ.Events;
using BinaryDiff.Worker.App.Events.IntegrationEvents;
using BinaryDiff.Worker.Domain.Logic;
using BinaryDiff.Worker.Domain.Models;
using BinaryDiff.Worker.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BinaryDiff.Worker.App.Events.IntegrationEventHandlers
{
    public class NewInputIntegrationEventHandler : IIntegrationEventHandler<NewInputIntegrationEvent>
    {
        private readonly ILogger _logger;
        private readonly IRabbitMQEventBus _eventBus;
        private readonly IInputRepository _inputRepository;
        private readonly IDiffLogic _logic;

        public NewInputIntegrationEventHandler(
            ILogger<NewInputIntegrationEventHandler> logger,
            IRabbitMQEventBus eventBus,
            IInputRepository inputRepository,
            IDiffLogic logic
        )
        {
            _logger = logger;
            _eventBus = eventBus;
            _inputRepository = inputRepository;
            _logic = logic;
        }

        /// <summary>
        /// Get opposing side input, compares them and publish result as NewResultIntegrationEvent
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public Task Handle(NewInputIntegrationEvent @event)
        {
            try
            {
                var (input, opposingInput) = GetInputsToCompare(@event);

                if (input != null)
                {
                    var diffResult = _logic.CompareData(input, opposingInput);

                    PublishNewResult(input, opposingInput?.Id, diffResult);
                }

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to process event {@event.Id}");
                throw;
            }
        }

        /// <summary>
        /// Determines 
        /// </summary>
        /// <param name="newInputEvent"></param>
        /// <returns></returns>
        private (InputData, InputData) GetInputsToCompare(NewInputIntegrationEvent newInputEvent)
        {
            var opposingPosition = _logic.GetOpposingPosition(newInputEvent.Position);

            var findInputTask = _inputRepository.FindAsync(newInputEvent.InputId);

            var findOpposingInputTask = _inputRepository
                .GetLastInputBeforeAsync(newInputEvent.DiffId, opposingPosition, newInputEvent.Timestamp);

            Task.WaitAll(findInputTask, findOpposingInputTask);

            return (findInputTask.Result, findOpposingInputTask.Result);
        }

        private void PublishNewResult(InputData input, string oppsingInputId, DiffResult diffResult)
        {
            var newResult = new NewResultIntegrationEvent(input, oppsingInputId, diffResult);

            _eventBus.Publish(newResult);
        }
    }
}
