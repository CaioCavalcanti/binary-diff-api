using AutoMapper;
using BinaryDiff.Input.Domain.Enums;
using BinaryDiff.Input.Domain.Models;
using BinaryDiff.Input.Infrastructure.Repositories;
using BinaryDiff.Input.WebApi.IntegrationEvents;
using BinaryDiff.Input.WebApi.ViewModels;
using BinaryDiff.Shared.Infrastructure.RabbitMQ.EventBus;
using BinaryDiff.Shared.WebApi.ResultMessages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BinaryDiff.Input.WebApi.Controllers
{
    [Route("api/diffs")]
    [ApiController]
    [Produces("application/json")]
    public class DiffController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IDiffRepository _diffRepository;
        private readonly IInputRepository _inputRepository;
        private readonly IMapper _mapper;
        private readonly IRabbitMQEventBus _eventBus;

        public DiffController(
            ILogger<DiffController> logger,
            IDiffRepository diffRepository,
            IInputRepository inputRepository,
            IMapper mapper,
            IRabbitMQEventBus eventBus
        )
        {
            _logger = logger;
            _diffRepository = diffRepository;
            _inputRepository = inputRepository;
            _mapper = mapper;
            _eventBus = eventBus;
        }

        /// <summary>
        /// Adds a new diff to repository and return its id
        /// </summary>
        /// <returns>DiffViewModel</returns>
        [HttpPost]
        [ProducesResponseType(typeof(DiffViewModel), 201)]
        [ProducesResponseType(typeof(ExceptionResultMessage), 500)]
        public async Task<IActionResult> PostAsync()
        {
            _logger.LogInformation("New diff request");

            var newDiff = new Diff();

            _logger.LogInformation("Adding diff...");

            await _diffRepository.AddOneAsync(newDiff);

            _logger.LogInformation($"Diff added ({newDiff.UUID})");

            var viewModel = _mapper.Map<DiffViewModel>(newDiff);

            return Created(newDiff.UUID.ToString(), viewModel);
        }

        /// <summary>
        /// Adds data to left position of a diff of given id
        /// </summary>
        /// <param name="id">From route - Guid diff unique identifier</param>
        /// <param name="input">From body - Data to be added on position</param>
        /// <returns>InputCreatedViewModel</returns>
        [HttpPost("{id}/left")]
        [ProducesResponseType(typeof(InputViewModel), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(ResourceNotFoundForIdResultMessage<Diff>), 404)]
        [ProducesResponseType(typeof(ExceptionResultMessage), 500)]
        public async Task<IActionResult> PostLeftAsync([FromRoute]Guid id, [FromBody]NewInputViewModel input)
        {
            return await HandlePostInputRequest(id, input, InputPosition.Left);
        }

        /// <summary>
        /// Adds data to right position of a diff of given id
        /// </summary>
        /// <param name="id">From route - Guid diff unique identifier</param>
        /// <param name="input">From body - Data to be added on position</param>
        /// <returns>InputCreatedViewModel</returns>
        [HttpPost("{id}/right")]
        [ProducesResponseType(typeof(InputViewModel), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(ResourceNotFoundForIdResultMessage<Diff>), 404)]
        [ProducesResponseType(typeof(ExceptionResultMessage), 500)]
        public async Task<IActionResult> PostRightAsync([FromRoute]Guid id, [FromBody]NewInputViewModel input)
        {
            return await HandlePostInputRequest(id, input, InputPosition.Right);
        }

        private async Task<IActionResult> HandlePostInputRequest(Guid diffId, NewInputViewModel input, InputPosition position)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Post{Enum.GetName(typeof(InputPosition), position)}({diffId}) failed validation");

                return BadRequest(ModelState);
            }

            _logger.LogInformation($"Find({diffId}): retrieving item on repository");

            var diff = await _diffRepository.FindAsync(_ => _.UUID == diffId);

            if (diff == null)
            {
                _logger.LogWarning($"Find({diffId}): item not found on repository");

                return NotFound(new ResourceNotFoundForIdResultMessage<Diff>(diffId));
            }

            var newInput = new InputData(diffId, position, input.Data);

            _logger.LogInformation($"Save({diffId}, Diff obj): saving item on repository");

            await _inputRepository.AddOneAsync(newInput);

            _logger.LogInformation($"Save({diffId}, Diff obj): saving item on repository");

            var eventMessage = new NewInputIntegrationEvent(newInput);

            _logger.LogInformation($"Input registered, sending integration event ({eventMessage.Id})");

            _eventBus.Publish(eventMessage);

            _logger.LogInformation($"Integration event ({eventMessage.Id}) sent!");

            var viewModel = _mapper.Map<InputViewModel>(newInput);

            return Created(newInput.Id, viewModel);
        }
    }
}