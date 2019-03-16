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
    public class ApiTests : IClassFixture<WebApplicationFactory<BinaryDiff.API.Startup>>
    {
        private readonly HttpClient _client;

        public ApiTests(WebApplicationFactory<BinaryDiff.API.Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task PostDiff_CreatesDiff_Returns201NewDiff()
        {
            var response = await _client.PostAsync("/v1/diff", new StringContent(string.Empty));

            var createdDiff = JsonConvert.DeserializeObject<DiffViewModel>(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(createdDiff);
            Assert.NotEqual(createdDiff.Id, Guid.Empty);
        }

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

        [Theory]
        [InlineData("1")]
        [InlineData("SGVsbG8gV29ybGQh")]
        public async Task PostLeft_IfInvalidId_Returns400ModelErrors(string id)
        {
            var response = await _client.PostAsync($"/v1/diff/{id}/left", PrepareData());

            await AssertBadRequestForInvalidIdAsync(id, response);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("SGVsbG8gV29ybGQh")]
        public async Task PostRight_IfInvalidId_Returns400ModelErrors(string id)
        {
            var response = await _client.PostAsync($"/v1/diff/{id}/right", PrepareData());

            await AssertBadRequestForInvalidIdAsync(id, response);
        }

        [Fact]
        public async Task PostLeft_IfIdNotFound_Returns404DiffNotFoundMessage()
        {
            // Act
            var response = await _client.PostAsync($"/v1/diff/{NotFoundDiffId}/left", PrepareData());

            // Assert
            await AssertNotFoundMessageAsync(response);
        }

        [Fact]
        public async Task PostRight_IfIdNotFound_Returns404DiffNotFoundMessage()
        {
            // Act
            var response = await _client.PostAsync($"/v1/diff/{NotFoundDiffId}/right", PrepareData());

            // Assert
            await AssertNotFoundMessageAsync(response);
        }

        [Fact]
        public async Task GetDiffResult_IfIdNotFound_Returns404DiffNotFoundMessage()
        {
            // Act
            var response = await _client.GetAsync($"/v1/diff/{NotFoundDiffId}");

            // Assert
            await AssertNotFoundMessageAsync(response);
        }

        [Theory]
        [InlineData(null, null, ResultType.AreEqual)]
        [InlineData("", "", ResultType.AreEqual)]
        [InlineData("SGVsbG8gV29ybGQh", "SGVsbG8gV29ybGQh", ResultType.AreEqual)]
        [InlineData("SGVsbG8gV29ybGQh", null, ResultType.LeftIsLarger)]
        [InlineData("SGVsbG8gV29ybGQh", "SGVsbG8h", ResultType.LeftIsLarger)]
        [InlineData(null, "SGVsbG8gV29ybGQh", ResultType.RightIsLarger)]
        [InlineData("SGVsbG8h", "SGVsbG8gV29ybGQh", ResultType.RightIsLarger)]
        [InlineData("SGVsbG8h", "SGVMTG8h", ResultType.DifferentContent)]
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
            var expectedMessage = $"We couldn't find a diff with ID '{NotFoundDiffId}'";

            var content = await response.Content.ReadAsStringAsync();
            var notFoundResult = JsonConvert.DeserializeObject<DiffNotFoundMessage>(content);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.NotNull(notFoundResult);
            Assert.Equal(expectedMessage, notFoundResult.Message);
        }

        private static string NotFoundDiffId => "00000000-0000-0000-0000-000000000000";

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
    }
}
