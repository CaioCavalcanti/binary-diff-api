using BinaryDiff.API.Helpers.Messages;
using BinaryDiff.API.ViewModels;
using BinaryDiff.Domain.Enum;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BinaryDiff.Tests.Integration
{
    /// <summary>
    /// API integration tests on happy-path and common issues
    /// </summary>
    public class ApiTests : IClassFixture<WebApplicationFactory<BinaryDiff.API.Startup>>
    {
        private readonly HttpClient _client;

        /// <summary>
        /// Create a new client instance for testing
        /// </summary>
        /// <param name="factory"></param>
        public ApiTests(WebApplicationFactory<BinaryDiff.API.Startup> factory)
        {
            _client = factory.CreateClient();
        }

        /// <summary>
        /// Tests created result when adding new result
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostDiff_CreatesDiff_Returns201NewDiff()
        {
            var response = await _client.PostAsync("/v1/diff", new StringContent(string.Empty));

            var createdDiff = JsonConvert.DeserializeObject<DiffViewModel>(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(createdDiff);
            Assert.NotEqual(createdDiff.Id, Guid.Empty);
        }

        /// <summary>
        /// Tests created result when adding new input on left
        /// </summary>
        /// <param name="data">Possible data to be sent</param>
        /// <returns></returns>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("SGVsbG8gV29ybGQh")]
        public async Task PostLeft_AddsDataToDiffLeft_Returns201EmptyBody(string data)
        {
            // Arrange
            var newDiffResponse = await _client.PostAsync("/v1/diff", new StringContent(string.Empty));
            var createdDiff = JsonConvert.DeserializeObject<DiffViewModel>(await newDiffResponse.Content.ReadAsStringAsync());

            var leftData = PrepareData(data);

            // Act
            var result = await _client.PostAsync($"/v1/diff/{createdDiff.Id.ToString()}/left", leftData);
            var content = await result.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
            Assert.Equal(string.Empty, content);
        }

        /// <summary>
        /// Tests created result when adding new input on left
        /// </summary>
        /// <param name="data">Possible data to be sent</param>
        /// <returns></returns>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("SGVsbG8gV29ybGQh")]
        public async Task PostRight_AddsDataToDiffRight_Returns201EmptyBody(string data)
        {
            // Arrange
            var newDiffResponse = await _client.PostAsync("/v1/diff", new StringContent(string.Empty));
            var createdDiff = JsonConvert.DeserializeObject<DiffViewModel>(await newDiffResponse.Content.ReadAsStringAsync());

            var leftData = PrepareData(data);

            // Act
            var result = await _client.PostAsync($"/v1/diff/{createdDiff.Id.ToString()}/right", leftData);
            var content = await result.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
            Assert.Equal(string.Empty, content);
        }

        /// <summary>
        /// Tests bad request result for invalid id
        /// </summary>
        /// <param name="id">Invalid id</param>
        /// <returns></returns>
        [Theory]
        [InlineData("1")]
        [InlineData("SGVsbG8gV29ybGQh")]
        public async Task PostLeft_Returns400ModelErrors_IfInvalidId(string id)
        {
            var response = await _client.PostAsync($"/v1/diff/{id}/left", PrepareData());

            await AssertBadRequestForInvalidIdAsync(id, response);
        }

        /// <summary>
        /// Tests bad request result for invalid id
        /// </summary>
        /// <param name="id">Invalid id</param>
        /// <returns></returns>
        [Theory]
        [InlineData("1")]
        [InlineData("SGVsbG8gV29ybGQh")]
        public async Task PostRight_Returns400ModelErrors_IfInvalidId(string id)
        {
            var response = await _client.PostAsync($"/v1/diff/{id}/right", PrepareData());

            await AssertBadRequestForInvalidIdAsync(id, response);
        }

        /// <summary>
        /// Tests not found result for a key that doesn't exist
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostLeft_Returns404DiffNotFoundMessage_IfIdNotFound()
        {
            // Act
            var response = await _client.PostAsync($"/v1/diff/{_notFoundId}/left", PrepareData());

            // Assert
            await AssertNotFoundMessageAsync(response);
        }

        /// <summary>
        /// Tests not found result for a key that doesn't exist
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostRight_Returns404DiffNotFoundMessage_IfIdNotFound()
        {
            // Act
            var response = await _client.PostAsync($"/v1/diff/{_notFoundId}/right", PrepareData());

            // Assert
            await AssertNotFoundMessageAsync(response);
        }

        /// <summary>
        /// Tests not found result for a key that doesn't exist
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetDiffResult_Returns404DiffNotFoundMessage_IfIdNotFound()
        {
            // Act
            var response = await _client.GetAsync($"/v1/diff/{_notFoundId}");

            // Assert
            await AssertNotFoundMessageAsync(response);
        }

        /// <summary>
        /// Tests diff result for different possible scenarios, with expected results
        /// </summary>
        /// <param name="left">Left input</param>
        /// <param name="right">Right input</param>
        /// <param name="expectedResult">Expected scenario</param>
        /// <returns></returns>
        [Theory]
        [InlineData(null, null, ResultType.Equal)]
        [InlineData("", "", ResultType.Equal)]
        [InlineData("SGVsbG8gV29ybGQh", "SGVsbG8gV29ybGQh", ResultType.Equal)]
        [InlineData("SGVsbG8gV29ybGQh", null, ResultType.LeftIsLarger)]
        [InlineData("SGVsbG8gV29ybGQh", "SGVsbG8h", ResultType.LeftIsLarger)]
        [InlineData(null, "SGVsbG8gV29ybGQh", ResultType.RightIsLarger)]
        [InlineData("SGVsbG8h", "SGVsbG8gV29ybGQh", ResultType.RightIsLarger)]
        [InlineData("SGVsbG8h", "SGVMTG8h", ResultType.Different)]
        public async Task GetDiffResult_Returns200DiffResultsViewModel(string left, string right, ResultType expectedResult)
        {
            // Arrange
            var newDiffResponse = await _client.PostAsync("/v1/diff", new StringContent(string.Empty));
            var createdDiff = JsonConvert.DeserializeObject<DiffViewModel>(await newDiffResponse.Content.ReadAsStringAsync());

            await Task.WhenAll(new Task[]
            {
                _client.PostAsync($"/v1/diff/{createdDiff.Id.ToString()}/left", PrepareData(left)),
                _client.PostAsync($"/v1/diff/{createdDiff.Id.ToString()}/right", PrepareData(right)),
            });

            // Act
            var response = await _client.GetAsync($"/v1/diff/{createdDiff.Id.ToString()}");
            var strResult = await response.Content.ReadAsStringAsync();
            var diffResult = JsonConvert.DeserializeObject<DiffResultViewModel>(strResult);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(Enum.GetName(typeof(ResultType), expectedResult), diffResult.Result);
        }


        private async Task AssertBadRequestForInvalidIdAsync(string id, HttpResponseMessage response)
        {
            // Arrange
            var expectedMessage = new { id = new string[] { $"The value '{id}' is not valid." } };

            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(JsonConvert.SerializeObject(expectedMessage), content);

        }

        private async Task AssertNotFoundMessageAsync(HttpResponseMessage response)
        {
            var expectedMessage = $"We couldn't find a diff with ID '{_notFoundId}'";

            var content = await response.Content.ReadAsStringAsync();
            var notFoundResult = JsonConvert.DeserializeObject<DiffNotFoundMessage>(content);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.NotNull(notFoundResult);
            Assert.Equal(expectedMessage, notFoundResult.Message);
        }

        private StringContent PrepareData(string data = null)
        {
            var serializedInput = JsonConvert.SerializeObject(
                new DiffInputViewModel { Data = data },
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Include
                });

            return new StringContent(serializedInput, Encoding.UTF8, "application/json");
        }

        private static string _notFoundId => "00000000-0000-0000-0000-000000000000";
    }
}
