using AutoMapper;
using BinaryDiff.API.Helpers.Messages;
using BinaryDiff.API.ViewModels;
using BinaryDiff.Domain.Enum;
using BinaryDiff.Domain.Logic;
using BinaryDiff.Domain.Models;
using BinaryDiff.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;

namespace BinaryDiff.API.Controllers
{
    /// <summary>
    /// v1 endpoints for Diff resource
    /// </summary>
    [Produces("application/json")]
    [Route("v1/diff")]
    [ApiController]
    public class DiffController : ControllerBase
    {
        private readonly IMemoryRepository<Guid, Diff> _diffRepository;
        private readonly IDiffLogic _logic;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        /// <summary>
        /// Injects types necessary for controller
        /// </summary>
        /// <param name="diffRepository"></param>
        /// <param name="logic"></param>
        /// <param name="mapper"></param>
        /// <param name="logger"></param>
        public DiffController(
            IMemoryRepository<Guid, Diff> diffRepository,
            IDiffLogic logic,
            IMapper mapper,
            ILogger<DiffController> logger
        )
        {
            _diffRepository = diffRepository;
            _logic = logic;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Generates a new diff entity
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(DiffViewModel), 201)]
        [ProducesResponseType(typeof(ExceptionMessage), 500)]
        [ProducesResponseType(typeof(DiffViewModel), 201)]
        public IActionResult PostDiff()
        {
            _logger.LogDebug("New diff requested");

            var newDiff = new Diff();

            _diffRepository.Save(newDiff.Id, newDiff);

            _logger.LogDebug("New diff created", newDiff.Id);

            // TODO: send new diff event

            return Created($"/v1/diff/{newDiff.Id.ToString()}", _mapper.Map<DiffViewModel>(newDiff));
        }

        /// <summary>
        /// Insert base64 data on left position of a diff entity for given id
        /// </summary>
        /// <param name="id">Guid identifier of diff entity</param>
        /// <param name="input">Base 64 encoded binary data</param>
        /// <returns></returns>
        [HttpPost("{id}/left")]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(ModelStateDictionary), 400)]
        [ProducesResponseType(typeof(DiffNotFoundMessage), 404)]
        [ProducesResponseType(typeof(ExceptionMessage), 500)]
        public IActionResult PostLeft([FromRoute]Guid id, [FromBody]DiffInputViewModel input)
        {
            return HandlePostInputOn(id, Position.Left, input);
        }

        /// <summary>
        /// Insert base64 data on right position of a diff entity for given id
        /// </summary>
        /// <param name="id">Guid identifier of diff entity</param>
        /// <param name="input">Base 64 encoded binary data</param>
        /// <returns></returns>
        [HttpPost("{id}/right")]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(ModelStateDictionary), 400)]
        [ProducesResponseType(typeof(DiffNotFoundMessage), 404)]
        [ProducesResponseType(typeof(ExceptionMessage), 500)]
        public IActionResult PostRight([FromRoute]Guid id, [FromBody]DiffInputViewModel input)
        {
            return HandlePostInputOn(id, Position.Right, input);
        }

        /// <summary>
        /// Returns a diff result for data provided on left and right positions
        /// </summary>
        /// <param name="id">Guid identifier of diff entity</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DiffResultViewModel), 200)]
        [ProducesResponseType(typeof(ModelStateDictionary), 400)]
        [ProducesResponseType(typeof(DiffNotFoundMessage), 404)]
        [ProducesResponseType(typeof(ExceptionMessage), 500)]
        public IActionResult GetDiffResult([FromRoute]Guid id)
        {
            _logger.LogDebug($"GetDiffResult({id})");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"GetDiffResult({id}) failed validation");
                return BadRequest(ModelState);
            }

            _logger.LogDebug($"Find({id}): retrieving item on repository");
            var diff = _diffRepository.Find(id);

            if (diff == null)
            {
                _logger.LogWarning($"Find({id}): item not found on repository");
                return NotFound(new DiffNotFoundMessage(id));
            }

            // TODO: has it changed since last call?

            _logger.LogDebug($"GetResult({id}): Calculating results");
            var diffResult = _logic.GetResult(diff);

            // TODO: return equal
            // TODO: return not equal size
            // TODO: return offset + length if same size

            _logger.LogDebug($"Diff result for {id}: {Enum.GetName(typeof(ResultType), diffResult.Result)}");
            return Ok(_mapper.Map<DiffResultViewModel>(diffResult));
        }

        private IActionResult HandlePostInputOn(Guid diffId, Position position, DiffInputViewModel input)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Post{Enum.GetName(typeof(Position), position)}({diffId}) failed validation");
                return BadRequest(ModelState);
            }

            _logger.LogDebug($"Find({diffId}): retrieving item on repository");
            var diff = _diffRepository.Find(diffId);

            if (diff == null)
            {
                _logger.LogWarning($"Find({diffId}): item not found on repository");
                return NotFound(new DiffNotFoundMessage(diffId));
            }

            // TODO: send new input event

            _logger.LogDebug($"Putting data (size: {input.Data?.Length}) on {Enum.GetName(typeof(Position), position)} position of {diffId}");
            if (position == Position.Left)
            {
                diff.Left = input.Data;
            }
            else if (position == Position.Right)
            {
                diff.Right = input.Data;
            }

            _logger.LogDebug($"Save({diffId}, Diff obj): saving item on repository");
            _diffRepository.Save(diff.Id, diff);

            return Created($"/v1/diff/{diff.Id.ToString()}", null);
        }
    }
}