using BinaryDiff.Domain.Logic;
using BinaryDiff.Domain.Models;
using BinaryDiff.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BinaryDiff.API.Controllers
{
    [Route("v1/diff")]
    [ApiController]
    public class DiffController : ControllerBase
    {
        private readonly IMemoryRepository<Guid, DiffModel> _diffRepository;
        private readonly ILeftRepository _leftRepository;
        private readonly IRightRepository _rightRepository;
        private readonly IDiffLogic _logic;

        public DiffController(
            IMemoryRepository<Guid, DiffModel> diffRepository,
            ILeftRepository leftRepository,
            IRightRepository rightRepository,
            IDiffLogic logic
        )
        {
            _diffRepository = diffRepository;
            _leftRepository = leftRepository;
            _rightRepository = rightRepository;
            _logic = logic;
        }

        [HttpPost]
        public async Task<IActionResult> PostDiff()
        {
            var newDiff = new DiffModel();

            _diffRepository.Save(newDiff.Id, newDiff);

            // TODO: send new diff event

            return Created($"/v1/diff/{newDiff.Id.ToString()}", newDiff);
        }

        [HttpPost("{id}/left")]
        public async Task<IActionResult> PostLeftAsync([FromRoute]Guid id, [FromBody]string strBase64)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // TODO: does id exist?
            // TODO: conflicts?
            // TODO: is base64?
            // TODO: send new left event

            _leftRepository.Save(id, strBase64);

            return Ok();
        }

        [HttpPost("{id}/right")]
        public async Task<IActionResult> PostRightAsync([FromRoute]Guid id, [FromBody]string strBase64)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // TODO: does id exist?
            // TODO: conflicts?
            // TODO: is base64?
            // TODO: send new right event

            _rightRepository.Save(id, strBase64);

            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDiffAsync([FromRoute]Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // TODO: does id exists?
            // TODO: get diff

            // TODO: does it have right?
            // TODO: does it have left?

            // TODO: has it changed since last call?

            var left = _leftRepository.Find(id);
            var right = _rightRepository.Find(id);

            var diffResult = _logic.GetDiffResult(left, right);

            // TODO: return equal
            // TODO: return not equal size
            // TODO: return offset + length if same size

            return Ok(diffResult);
        }

        private void ValidateId(Guid id)
        {
            if (id == null || id == Guid.Empty)
            {
                ModelState.AddModelError("Id", "Id informed is not valid");
            }
        }
    }
}