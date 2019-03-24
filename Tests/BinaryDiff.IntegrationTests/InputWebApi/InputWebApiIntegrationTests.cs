using BinaryDiff.Input.Domain.Enums;
using BinaryDiff.Input.WebApi;
using BinaryDiff.Input.WebApi.ViewModels;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BinaryDiff.IntegrationTests.InputWebApi
{
    public class InputWebApiIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public InputWebApiIntegrationTests(WebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task PostDiff_creates_diff_returns_created_result()
        {
            // Act
            var response = await _client.PostAsync("/api/diffs", null);

            var strContent = await response.Content.ReadAsStringAsync();
            var diff = JsonConvert.DeserializeObject<DiffViewModel>(strContent);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(diff);
            Assert.NotEqual(Guid.Empty, diff.Id);
        }

        [Fact]
        public async Task PostLeft_adds_data_to_left_position_on_diff_and_returns_created_result()
        {
            // Arrange
            var newDiff = await GetNewDiff();

            // Act
            var leftData = new NewInputViewModel("Zm9vYmFy");
            var payload = new StringContent(JsonConvert.SerializeObject(leftData), Encoding.UTF8, "application/json");
            var leftInputResponse = await _client.PostAsync($"/api/diffs/{newDiff.Id}/left", payload);

            // Assert
            await AssertInputCreatedOnDiffPosition(leftInputResponse, newDiff.Id, InputPosition.Left);
        }

        [Fact]
        public async Task PostRight_adds_data_to_right_position_on_diff_and_returns_created_result()
        {
            // Arrange
            var newDiff = await GetNewDiff();

            // Act
            var leftData = new NewInputViewModel("Zm9vYmFy");
            var payload = new StringContent(JsonConvert.SerializeObject(leftData), Encoding.UTF8, "application/json");
            var rightInputResponse = await _client.PostAsync($"/api/diffs/{newDiff.Id}/right", payload);

            // Assert
            await AssertInputCreatedOnDiffPosition(rightInputResponse, newDiff.Id, InputPosition.Right);
        }

        private async Task AssertInputCreatedOnDiffPosition(HttpResponseMessage response, Guid diffId, InputPosition position)
        {
            var strInputContent = await response.Content.ReadAsStringAsync();
            var inputResult = JsonConvert.DeserializeObject<InputViewModel>(strInputContent);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(inputResult);
            Assert.Equal(diffId, inputResult.DiffId);
            Assert.Equal(Enum.GetName(typeof(InputPosition), position), inputResult.Position);
            Assert.NotEqual(default(DateTime), inputResult.Timestamp);
        }

        private async Task<DiffViewModel> GetNewDiff()
        {
            var newDiffResponse = await _client.PostAsync("/api/diffs", null);
            var strNewDiffContent = await newDiffResponse.Content.ReadAsStringAsync();
            var newDiff = JsonConvert.DeserializeObject<DiffViewModel>(strNewDiffContent);

            return newDiff;
        }
    }
}
