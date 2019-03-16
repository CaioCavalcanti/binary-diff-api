using BinaryDiff.API.ViewModels;
using BinaryDiff.Domain.Enum;
using BinaryDiff.Domain.Logic;
using BinaryDiff.Domain.Models;
using BinaryDiff.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BinaryDiff.API.Controllers
{
    [Produces("application/json")]
    [Route("v1/diff")]
    [ApiController]
    public class DiffController : ControllerBase
    {
        private readonly IMemoryRepository<Guid, Diff> _diffRepository;
        private readonly IDiffLogic _logic;

        public DiffController(
            IMemoryRepository<Guid, Diff> diffRepository,
            IDiffLogic logic
        )
        {
            _diffRepository = diffRepository;
            _logic = logic;
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostDiff()
        {
            var newDiff = new Diff();

            _diffRepository.Save(newDiff.Id, newDiff);

            // TODO: send new diff event

            return Created($"/v1/diff/{newDiff.Id.ToString()}", newDiff);
        }

        [HttpPost("{id}/left")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostLeftAsync([FromRoute]Guid id, [FromBody]DiffInputViewModel input)
        {
            return HandlePostInputOn(id, DiffDirection.Left, input);
        }

        [HttpPost("{id}/right")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostRightAsync([FromRoute]Guid id, [FromBody]DiffInputViewModel input)
        {
            return HandlePostInputOn(id, DiffDirection.Right, input);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetDiffAsync([FromRoute]Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var aDiff = _diffRepository.Find(id);

            if (aDiff == null)
            {
                return NotFound();
            }

            // TODO: does id exists?
            // TODO: get diff

            // TODO: does it have right?
            // TODO: does it have left?

            // TODO: has it changed since last call?

            var diffResult = _logic.GetResultFor(aDiff, out var diffDetails);

            // TODO: return equal
            // TODO: return not equal size
            // TODO: return offset + length if same size

            return Ok(new { diffResult, diffDetails });
        }

        private void ValidateId(Guid id)
        {
            if (id == null || id == Guid.Empty)
            {
                ModelState.AddModelError("Id", "Id informed is not valid");
            }
        }

        private IActionResult HandlePostInputOn(Guid id, DiffDirection direction, DiffInputViewModel input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var diff = _diffRepository.Find(id);

            if (diff == null)
            {
                return NotFound();
            }

            // TODO: conflicts?
            // TODO: send new input event

            _logic.AddInput(diff, direction, input.Data);

            _diffRepository.Save(diff.Id, diff);

            return Created($"/v1/diff/{diff.Id.ToString()}", null);
        }
    }
}