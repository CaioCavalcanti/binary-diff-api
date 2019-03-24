using BinaryDiff.Result.Domain.Models;
using BinaryDiff.Result.Infrastructure.Database;
using BinaryDiff.Result.Infrastructure.Repositories.Implementation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace BinaryDiff.Result.UnitTests.Infrastructure
{
    public class DiffResultsRepositoryTests
    {
        /// <summary>
        /// Tests if it returns the last diff result for a given id based on the timestamp
        /// result timestamp
        /// </summary>
        [Fact]
        public async Task GetLastResultForDiffAsync_returns_last_result_for_a_given_diff_id()
        {
            // Arrange
            var expectedResult = new DiffResult
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Parse("2019-01-01")
            };

            var options = new DbContextOptionsBuilder<ResultContext>()
                .UseInMemoryDatabase("resultsDb_lastResult")
                .Options;
            var context = new ResultContext(options);
            var repository = new DiffResultsRepository(context);

            context.DiffResults.AddRange(new DiffResult[] {
                new DiffResult { Id = Guid.NewGuid(), DiffId = expectedResult.DiffId, Timestamp = expectedResult.Timestamp.AddMilliseconds(-1) },
                expectedResult,
                new DiffResult { Id = Guid.NewGuid(), DiffId = expectedResult.DiffId, Timestamp = expectedResult.Timestamp.AddHours(-1) },
                new DiffResult { Id = Guid.NewGuid(), DiffId = expectedResult.DiffId, Timestamp = expectedResult.Timestamp.AddMinutes(-1) },
                new DiffResult { Id = Guid.NewGuid(), DiffId = expectedResult.DiffId, Timestamp = expectedResult.Timestamp.AddSeconds(-1) },
                new DiffResult { Id = Guid.NewGuid(), DiffId = Guid.NewGuid(), Timestamp = expectedResult.Timestamp }
            });
            context.SaveChanges();

            // Act
            var result = await repository.GetLastResultForDiffAsync(expectedResult.DiffId);

            // Assert
            Assert.Equal(result.Id, expectedResult.Id);
            Assert.Equal(result.Timestamp, expectedResult.Timestamp);
            Assert.Equal(result.DiffId, expectedResult.DiffId);
        }

        /// <summary>
        /// Tests if it returns the related differences, if any
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetLastResultForDiffAsync_includes_differences()
        {
            // Arrange
            var expectedResult = new DiffResult
            {
                Id = Guid.NewGuid(),
                Differences = new InputDifference[] {
                    new InputDifference(1, 2),
                    new InputDifference(5, 6),
                    new InputDifference(12, 4)
                }
            };

            var options = new DbContextOptionsBuilder<ResultContext>()
                .UseInMemoryDatabase("resultsDb_notEmpty")
                .Options;
            var context = new ResultContext(options);
            var repository = new DiffResultsRepository(context);

            context.DiffResults.Add(expectedResult);
            context.SaveChanges();

            // Act
            var result = await repository.GetLastResultForDiffAsync(expectedResult.DiffId);

            // Assert
            Assert.NotEmpty(result.Differences);
            Assert.All(result.Differences, diff => expectedResult.Differences.Contains(diff));
        }
    }
}
