using AutoMapper;
using BinaryDiff.Input.Domain.Enums;
using BinaryDiff.Input.Domain.Models;
using BinaryDiff.Input.Infrastructure.Repositories;
using BinaryDiff.Input.WebApi.Helpers.Messages;
using BinaryDiff.Input.WebApi.ViewModels;
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

        public DiffController(
            ILogger<DiffController> logger,
            IDiffRepository diffRepository,
            IInputRepository inputRepository,
            IMapper mapper
        )
        {
            _logger = logger;
            _diffRepository = diffRepository;
            _inputRepository = inputRepository;
            _mapper = mapper;
        }

        [HttpPost]
        [ProducesResponseType(typeof(DiffViewModel), 201)]
        [ProducesResponseType(typeof(ExceptionMessage), 500)]
        public async Task<IActionResult> Post()
        {
            _logger.LogDebug("New diff request");

            var newDiff = new Diff();

            _logger.LogDebug("Adding diff...");

            await _diffRepository.AddOneAsync(newDiff);

            _logger.LogDebug($"Diff added ({newDiff.UUID.ToString()})");

            var viewModel = _mapper.Map<DiffViewModel>(newDiff);

            return Created(newDiff.UUID.ToString(), viewModel);
        }

        [HttpPost("{id}/left")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(DiffNotFoundMessage), 404)]
        [ProducesResponseType(typeof(ExceptionMessage), 500)]
        public async Task<IActionResult> PostLeft([FromRoute]Guid id, [FromBody]InputViewModel input)
        {
            return await HandlePostInputRequest(id, input, InputPosition.Left);
        }

        [HttpPost("{id}/right")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(DiffNotFoundMessage), 404)]
        [ProducesResponseType(typeof(ExceptionMessage), 500)]
        public async Task<IActionResult> PostRight([FromRoute]Guid id, [FromBody]InputViewModel input)
        {
            return await HandlePostInputRequest(id, input, InputPosition.Right);
        }

        private async Task<IActionResult> HandlePostInputRequest(Guid diffId, InputViewModel input, InputPosition position)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Post{Enum.GetName(typeof(InputPosition), position)}({diffId}) failed validation");

                return BadRequest(ModelState);
            }

            _logger.LogDebug($"Find({diffId}): retrieving item on repository");

            var diff = await _diffRepository.FindAsync(_ => _.UUID == diffId);

            if (diff == null)
            {
                _logger.LogWarning($"Find({diffId}): item not found on repository");

                return NotFound(new DiffNotFoundMessage(diffId));
            }

            var newInput = new InputData(diffId, position, input.Data);

            _logger.LogDebug($"Save({diffId}, Diff obj): saving item on repository");
            await _inputRepository.AddOneAsync(newInput);

            return Created(diffId.ToString(), null);
        }
    }
}