using AutoMapper;
using BinaryDiff.Result.Domain.Enums;
using BinaryDiff.Result.Domain.Models;
using BinaryDiff.Result.Infrastructure.Repositories;
using BinaryDiff.Result.WebApi.Controllers;
using BinaryDiff.Result.WebApi.Mappers;
using BinaryDiff.Result.WebApi.ViewModels;
using BinaryDiff.Shared.WebApi.ResultMessages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BinaryDiff.Result.UnitTests.WebApi
{
    public class DiffControllerTests
    {
        private readonly IMapper _mapper;

        public DiffControllerTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DiffResultMapper>();
            });
            _mapper = config.CreateMapper();
        }

        /// <summary>
        /// Tests if GetAsync it returns the last result for a given id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetAsync_returns_last_result_for_a_given_diff_id()
        {
            // Arrange
            var expectedResult = new DiffResult
            {
                Id = Guid.NewGuid(),
                DiffId = Guid.NewGuid(),
                Result = ResultType.Different,
                Timestamp = DateTime.Now,
                Differences = new InputDifference[] { new InputDifference(1, 2), new InputDifference(5, 3) }
            };

            var logger = new Mock<ILogger<DiffController>>();
            var repository = new Mock<IDiffResultsRepository>();
            repository.Setup(s => s.GetLastResultForDiffAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedResult);

            var controller = new DiffController(logger.Object, _mapper, repository.Object);

            // Act
            var response = await controller.GetAsync(expectedResult.DiffId);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(response);
            var result = Assert.IsType<DiffResultViewModel>(okResult.Value);

            repository.Verify(r =>
                r.GetLastResultForDiffAsync(It.Is<Guid>(id => id == expectedResult.DiffId)),
                Times.Once
            );

            Assert.Equal(expectedResult.Id, result.Id);
            Assert.Equal(Enum.GetName(typeof(ResultType), expectedResult.Result), result.Result);
            Assert.Equal(expectedResult.Timestamp, result.Timestamp);
            Assert.Equal(expectedResult.Differences.Count, result.Differences.Count());
        }

        /// <summary>
        /// Tests if GetAsync returns ResourceNotFoundForIdResultMessage<DiffResult> if diff not found
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetAsync_returns_ResourceNotFoundForIdResultMessage_if_diff_id_not_found()
        {
            // Arrange
            var diffId = Guid.NewGuid();
            var logger = new Mock<ILogger<DiffController>>();
            var repository = new Mock<IDiffResultsRepository>();
            repository.Setup(s => s.GetLastResultForDiffAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => null);

            var controller = new DiffController(logger.Object, _mapper, repository.Object);

            // Act
            var response = await controller.GetAsync(diffId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(response);
            var result = Assert.IsType<ResourceNotFoundForIdResultMessage<DiffResult>>(notFoundResult.Value);
            Assert.Equal($"{nameof(DiffResult)} with ID '{diffId}' not found", result.Message);
        }
    }
}
