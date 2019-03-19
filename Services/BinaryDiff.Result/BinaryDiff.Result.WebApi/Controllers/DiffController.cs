using AutoMapper;
using BinaryDiff.Result.Domain.Enums;
using BinaryDiff.Result.Domain.Models;
using BinaryDiff.Result.Infrastructure.Repositories;
using BinaryDiff.Result.WebApi.ViewModels;
using BinaryDiff.Shared.WebApi.ResultMessages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BinaryDiff.Result.WebApi.Controllers
{
    [Route("api/diffs")]
    [ApiController]
    public class DiffController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public DiffController(
            ILogger<DiffController> logger,
            IMapper mapper,
            IUnitOfWork uow
        )
        {
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        [HttpGet("{diffId}")]
        [ProducesResponseType(typeof(DiffResultViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(ResourceNotFoundForIdResultMessage<DiffResult>), 404)]
        [ProducesResponseType(typeof(ExceptionResultMessage), 500)]
        public async Task<IActionResult> GetLastResultAsync([FromRoute]Guid diffId)
        {
            _logger.LogDebug($"Request to get last result for {diffId}");

            if (!ModelState.IsValid)
            {
                _logger.LogInformation($"Id provided {diffId} is not valid");

                return BadRequest(ModelState);
            }

            _logger.LogDebug($"Getting last result {diffId} on repository");

            var result = await _uow.DiffResultsRepository.GetLastResultForDiffAsync(diffId);

            if (result == null)
            {
                _logger.LogInformation($"None result found for {diffId}");

                return NotFound(new ResourceNotFoundForIdResultMessage<DiffResult>(diffId));
            }

            _logger.LogDebug($"Found result {result.Id} for {diffId}");

            var resultViewModel = _mapper.Map<DiffResultViewModel>(result);

            return Ok(resultViewModel);
        }

        [HttpPost("{diffId}")]
        [ProducesResponseType(typeof(NewDiffResultViewModel), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(ResourceNotFoundForIdResultMessage<DiffResult>), 404)]
        [ProducesResponseType(typeof(ExceptionResultMessage), 500)]
        public async Task<IActionResult> PostResultAsync([FromRoute]Guid diffId, [FromBody]NewDiffResultViewModel newResult)
        {
            _logger.LogDebug($"Request to add result for {diffId}");

            if (!ModelState.IsValid)
            {
                _logger.LogInformation($"Model for {diffId} is not valid");

                return BadRequest(ModelState);
            }

            if (newResult.Result == ResultType.Different &&
                (newResult.Differences == null || newResult.Differences.Keys.Count == 0))
            {
                ModelState.AddModelError("Differences", "Differences field is mandatory if result is different");

                _logger.LogInformation($"Model for {diffId} is not valid");

                return BadRequest(ModelState);
            }

            _logger.LogDebug($"Adding result to {diffId} on repository");

            var result = _mapper.Map<DiffResult>(newResult);
            result.DiffId = diffId;

            _uow.DiffResultsRepository.Add(result);

            await _uow.SaveChangesAsync();

            _logger.LogDebug($"Result added for {diffId}: {result.Id}");

            var resultViewModel = _mapper.Map<DiffResultViewModel>(result);

            return Created($"/diffs/{result.DiffId}/results/{result.Id}", resultViewModel);
        }
    }
}