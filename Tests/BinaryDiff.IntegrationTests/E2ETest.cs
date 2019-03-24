using BinaryDiff.Input.WebApi.ViewModels;
using BinaryDiff.Result.Domain.Enums;
using BinaryDiff.Result.WebApi.ViewModels;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BinaryDiff.IntegrationTests
{
    /// <summary>
    /// Run the E2E happy path on development environment. You'll need to run the containers to run it,
    /// as I couldn't put different TestServers to talk to each other (TODO).
    /// docker-compose -f docker-compose.yml -f docker-compose-infrastructure.yml up --build
    /// </summary>
    public class E2ETest
    {
        private readonly HttpClient _client;

        public E2ETest()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:4000")
            };
        }

        [Theory]
        [InlineData("Zm9vYmFyCg", "Zm9vYmFyCg", ResultType.Equal)]
        [InlineData("Zm9vYmFyCg", "Zm9v", ResultType.LeftIsLarger)]
        [InlineData("Zm9vYmFyCg", "", ResultType.LeftIsLarger)]
        [InlineData("Zm9vYmFyCg", null, ResultType.LeftIsLarger)]
        [InlineData("Zm9v", "Zm9vYmFyCg==", ResultType.RightIsLarger)]
        [InlineData("", "Zm9vYmFyCg==", ResultType.RightIsLarger)]
        [InlineData(null, "Zm9vYmFyCg==", ResultType.RightIsLarger)]
        [InlineData("Zm9vYmFy", "YmFyZm9v", ResultType.Different)]
        public async Task User_can_get_diff_results_for_given_inputs(string leftData, string rightData, ResultType expectedResult)
        {
            var newDiff = await CreateDiffAsync();

            var addLeftTask = AddInputToDiffAsync(ApiGatewayRoutes.Post.AddInputToLeft(newDiff.Id), leftData);
            var addRightTask = AddInputToDiffAsync(ApiGatewayRoutes.Post.AddInputToRight(newDiff.Id), rightData);

            Task.WaitAll(addLeftTask, addRightTask);

            var leftInput = addLeftTask.Result;
            var rightInput = addRightTask.Result;

            // gives 1s to get all results processed by RabbitMQ
            await Task.Delay(1000);

            var diffResult = await GetResultAsync(ApiGatewayRoutes.Get.DiffResult(newDiff.Id));

            Assert.Equal(Enum.GetName(typeof(ResultType), expectedResult), diffResult.Result);
        }

        private async Task<DiffViewModel> CreateDiffAsync()
        {
            var response = await _client.PostAsync(ApiGatewayRoutes.Post.CreateDiff, null);

            Assert.True(response.IsSuccessStatusCode, $"Failed to create new diff - {response.StatusCode}");

            var strContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<DiffViewModel>(strContent);
        }

        private async Task<InputViewModel> AddInputToDiffAsync(string route, string data)
        {
            if (string.IsNullOrEmpty(data)) return null;

            var newInput = new NewInputViewModel(data);
            var payload = new StringContent(JsonConvert.SerializeObject(newInput), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(route, payload);

            Assert.True(response.IsSuccessStatusCode, $"Failed to post on {route} - {response.StatusCode}");

            var strContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<InputViewModel>(strContent);
        }

        private async Task<DiffResultViewModel> GetResultAsync(string route)
        {
            var response = await _client.GetAsync(route);

            Assert.True(response.IsSuccessStatusCode, $"Failed to get diff result - {response.StatusCode}");

            var strContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<DiffResultViewModel>(strContent);
        }
    }
}
