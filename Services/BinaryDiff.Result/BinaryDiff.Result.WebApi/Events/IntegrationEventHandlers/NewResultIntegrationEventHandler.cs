using AutoMapper;
using BinaryDiff.Result.Domain.Models;
using BinaryDiff.Result.Infrastructure.Repositories;
using BinaryDiff.Result.WebApi.Events.IntegrationEvents;
using BinaryDiff.Shared.Infrastructure.RabbitMQ.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BinaryDiff.Result.WebApi.Events.IntegrationEventHandlers
{
    public class NewResultIntegrationEventHandler : IIntegrationEventHandler<NewResultIntegrationEvent>
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public NewResultIntegrationEventHandler(
            ILogger<NewResultIntegrationEventHandler> logger,
            IMapper mapper,
            IUnitOfWork uow
        )
        {
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// When a new result event is received, verifies if it was already registered on
        /// repository and adds it in case it doesn't exist
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public async Task Handle(NewResultIntegrationEvent @event)
        {
            try
            {
                _logger.LogInformation($"New {nameof(NewResultIntegrationEvent)} received: {@event.Id}");

                var resultExistsForInput = _uow.DiffResultsRepository
                    .Get(result => result.TriggerInputId == @event.InputId)
                    .Any();

                if (resultExistsForInput)
                {
                    _logger.LogInformation($"Result for input {@event.InputId} already registered on repository");
                    return;
                }

                var newResult = _mapper.Map<DiffResult>(@event);

                _uow.DiffResultsRepository.Add(newResult);

                await _uow.SaveChangesAsync();

                _logger.LogInformation($"Result saved on repository: {newResult.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to process {nameof(NewResultIntegrationEvent)}: {@event.Id}");
                throw;
            }
        }
    }
}
