using BinaryDiff.Shared.Infrastructure.RabbitMQ.Events;
using BinaryDiff.Worker.App.Events.IntegrationEvents;
using BinaryDiff.Worker.Domain.Logic;
using BinaryDiff.Worker.Domain.Models;
using BinaryDiff.Worker.Infrastructure.EventBus;
using BinaryDiff.Worker.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BinaryDiff.Worker.App.Events.IntegrationEventHandlers
{
    public class NewInputIntegrationEventHandler : IIntegrationEventHandler<NewInputIntegrationEvent>
    {
        private readonly ILogger _logger;
        private readonly IResultEventBus _resultEventBus;
        private readonly IInputRepository _inputRepository;
        private readonly IDiffLogic _logic;

        public NewInputIntegrationEventHandler(
            ILogger logger,
            IResultEventBus resultEventBus,
            IInputRepository inputRepository,
            IDiffLogic logic
        )
        {
            _logger = logger;
            _resultEventBus = resultEventBus;
            _inputRepository = inputRepository;
            _logic = logic;
        }

        public Task Handle(NewInputIntegrationEvent @event)
        {
            try
            {
                var (input, opposingInput) = GetInputsToCompare(@event);

                var diffResult = _logic.CompareData(input, opposingInput);

                PublishNewResult(input, opposingInput.Id, diffResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process");

                // TODO: handle exception (retry?)
            }

            return Task.CompletedTask;
        }

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

            _resultEventBus.Publish(newResult);
        }
    }
}
