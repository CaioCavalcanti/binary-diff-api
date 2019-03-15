using BinaryDiff.Domain.Logic;
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
        private readonly ILeftRepository _leftRepository;
        private readonly IRightRepository _rightRepository;
        private readonly IDiffLogic _logic;

        public DiffController(
            ILeftRepository leftRepository,
            IRightRepository rightRepository,
            IDiffLogic logic
        )
        {
            _leftRepository = leftRepository;
            _rightRepository = rightRepository;
            _logic = logic;
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