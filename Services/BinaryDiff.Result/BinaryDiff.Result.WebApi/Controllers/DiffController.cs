using AutoMapper;
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
        private readonly IDiffResultsRepository _repository;

        public DiffController(
            ILogger<DiffController> logger,
            IMapper mapper,
            IDiffResultsRepository repository
        )
        {
            _logger = logger;
            _mapper = mapper;
            _repository = repository;
        }

        /// <summary>
        /// Returns the last result registered for a given diff id
        /// </summary>
        /// <param name="diffId">Guid diff unique id</param>
        /// <returns></returns>
        [HttpGet("{diffId}")]
        [ProducesResponseType(typeof(DiffResultViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(ResourceNotFoundForIdResultMessage<DiffResult>), 404)]
        [ProducesResponseType(typeof(ExceptionResultMessage), 500)]
        public async Task<IActionResult> GetAsync([FromRoute]Guid diffId)
        {
            _logger.LogDebug($"Request to get last result for {diffId}");

            if (!ModelState.IsValid)
            {
                _logger.LogInformation($"Id provided {diffId} is not valid");

                return BadRequest(ModelState);
            }

            _logger.LogDebug($"Getting last result {diffId} on repository");

            var result = await _repository.GetLastResultForDiffAsync(diffId);

            if (result == null)
            {
                _logger.LogInformation($"None result found for {diffId}");

                return NotFound(new ResourceNotFoundForIdResultMessage<DiffResult>(diffId));
            }

            _logger.LogDebug($"Found result {result.Id} for {diffId}");

            var resultViewModel = _mapper.Map<DiffResultViewModel>(result);

            return new ObjectResult(resultViewModel);
        }
    }
}