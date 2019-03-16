using AutoMapper;
using BinaryDiff.API.Helpers.Messages;
using BinaryDiff.API.ViewModels;
using BinaryDiff.Domain.Enum;
using BinaryDiff.Domain.Logic;
using BinaryDiff.Domain.Models;
using BinaryDiff.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace BinaryDiff.API.Controllers
{
    [Produces("application/json")]
    [Route("v1/diff")]
    [ApiController]
    public class DiffController : ControllerBase
    {
        private readonly IMemoryRepository<Guid, Diff> _diffRepository;
        private readonly IDiffLogic _logic;
        private readonly IMapper _mapper;

        public DiffController(
            IMemoryRepository<Guid, Diff> diffRepository,
            IDiffLogic logic,
            IMapper mapper
        )
        {
            _diffRepository = diffRepository;
            _logic = logic;
            _mapper = mapper;
        }

        /// <summary>
        /// Generates a diff
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(ExceptionMessage), 500)]
        [ProducesResponseType(typeof(DiffViewModel), 201)]
        public IActionResult PostDiff()
        {
            var newDiff = new Diff();

            _diffRepository.Save(newDiff.Id, newDiff);

            // TODO: send new diff event

            return Created($"/v1/diff/{newDiff.Id.ToString()}", _mapper.Map<DiffViewModel>(newDiff));
        }

        [HttpPost("{id}/left")]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(ModelStateDictionary), 400)]
        [ProducesResponseType(typeof(DiffNotFoundMessage), 404)]
        [ProducesResponseType(typeof(ExceptionMessage), 500)]
        public IActionResult PostLeft([FromRoute]Guid id, [FromBody]DiffInputViewModel input)
        {
            return HandlePostInputOn(id, Position.Left, input);
        }

        [HttpPost("{id}/right")]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(ModelStateDictionary), 400)]
        [ProducesResponseType(typeof(DiffNotFoundMessage), 404)]
        [ProducesResponseType(typeof(ExceptionMessage), 500)]
        public IActionResult PostRight([FromRoute]Guid id, [FromBody]DiffInputViewModel input)
        {
            return HandlePostInputOn(id, Position.Right, input);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DiffResultViewModel), 200)]
        [ProducesResponseType(typeof(ModelStateDictionary), 400)]
        [ProducesResponseType(typeof(DiffNotFoundMessage), 404)]
        [ProducesResponseType(typeof(ExceptionMessage), 500)]
        public IActionResult GetDiffResult([FromRoute]Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var diff = _diffRepository.Find(id);

            if (diff == null)
            {
                return NotFound(new DiffNotFoundMessage(id));
            }

            // TODO: has it changed since last call?

            var diffResult = _logic.GetResultFor(diff);

            // TODO: return equal
            // TODO: return not equal size
            // TODO: return offset + length if same size

            return Ok(_mapper.Map<DiffResultViewModel>(diffResult));
        }

        private IActionResult HandlePostInputOn(Guid diffId, Position position, DiffInputViewModel input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var diff = _diffRepository.Find(diffId);

            if (diff == null)
            {
                return NotFound(new DiffNotFoundMessage(diffId));
            }

            // TODO: send new input event

            if (position == Position.Left)
            {
                diff.Left = input.Data;
            }
            else if (position == Position.Right)
            {
                diff.Right = input.Data;
            }

            _diffRepository.Save(diff.Id, diff);

            return Created($"/v1/diff/{diff.Id.ToString()}", null);
        }
    }
}