using BinaryDiff.Worker.App.IntegrationEvents;
using BinaryDiff.Worker.Domain.Logic;
using BinaryDiff.Worker.Domain.Models;
using BinaryDiff.Worker.Infrastructure.EventBus;
using BinaryDiff.Worker.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BinaryDiff.Worker.App
{
    public class Worker
    {
        private static AutoResetEvent _closing = new AutoResetEvent(false);

        private readonly ILogger _logger;
        private readonly IInputEventBus _inputEventBus;
        private readonly IResultEventBus _resultEventBus;
        private readonly IInputRepository _inputRepository;
        private readonly IDiffLogic _logic;

        public Worker(
            ILogger<Worker> logger,
            IInputEventBus inputEventBus,
            IResultEventBus resultEventBus,
            IInputRepository inputRepository,
            IDiffLogic logic
        )
        {
            _logger = logger;
            _inputEventBus = inputEventBus;
            _resultEventBus = resultEventBus;
            _inputRepository = inputRepository;
            _logic = logic;
        }

        public void ListenToNewInputs()
        {
            var queue = nameof(NewInputIntegrationEvent);

            _logger.LogInformation($"Opening channel to list to {queue} queue...");

            using (var channel = _inputEventBus.CreateChannel(queue))
            {
                _logger.LogInformation($"Listening to {channel}...");

                channel.BasicConsume(queue: queue,
                                     autoAck: true,
                                     consumer: GetConsumer(channel));

                _logger.LogInformation("Press CTRL + C to close the app");
                Console.CancelKeyPress += OnExit;
                _closing.WaitOne();
            }
        }

        private static void OnExit(object sender, ConsoleCancelEventArgs args)
        {
            Console.WriteLine("Exit");
            _closing.Set();
        }

        private EventingBasicConsumer GetConsumer(IModel channel)
        {
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (o, ea) => Task.Run(() => ProcessEventAsync(o, ea));

            return consumer;
        }

        async void ProcessEventAsync(object sender, BasicDeliverEventArgs ea)
        {
            try
            {
                _logger.LogInformation("New event received, processing message...");

                var newInputEvent = ReadNewInputFromMessage(ea);

                _logger.LogDebug("Getting input data to compare...");

                var (input, opposingInput) = await GetInputsToCompareAsync(newInputEvent);

                _logger.LogDebug($"Got inputs to compare ({input.Position}-{input.Id} / {opposingInput.Position}-{opposingInput.Id})");

                var diffResult = _logic.CompareData(input.Data, opposingInput?.Data);

                _logger.LogDebug($"Result: {diffResult.Result} (differences: {diffResult.Differences?.Count})");

                PublishNewResult(input, opposingInput.Id, diffResult);

                _logger.LogInformation("Event processed!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process");

                // TODO: handle exception (retry?)
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task<(InputData, InputData)> GetInputsToCompareAsync(NewInputIntegrationEvent newInputEvent)
        {
            _logger.LogDebug($"Getting opposing input to {newInputEvent.Position}");

            var opposingPosition = _logic.GetOpposingPosition(newInputEvent.Position);

            _logger.LogDebug($"Getting input data on repository {newInputEvent.InputId}");

            var findInputTask = _inputRepository.FindAsync(newInputEvent.InputId);

            _logger.LogDebug($"Getting opposing input to {newInputEvent.Position.ToString()}");

            var findOpposingInputTask = _inputRepository
                .GetLastInputBeforeAsync(newInputEvent.DiffId, opposingPosition, newInputEvent.Timestamp);

            Task.WaitAll(findInputTask, findOpposingInputTask);

            _logger.LogDebug($"Inputs retrieved from repository");

            return (findInputTask.Result, findOpposingInputTask.Result);
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

        private void PublishNewResult(InputData input, string oppsingInputId, DiffResult diffResult)
        {
            var newResult = new NewResultIntegrationEvent(input, oppsingInputId, diffResult);

            _resultEventBus.Publish(newResult);
        }

        private NewInputIntegrationEvent ReadNewInputFromMessage(BasicDeliverEventArgs ea)
        {
            _logger.LogDebug("Deserializing message...");

            var message = Encoding.UTF8.GetString(ea.Body);

            var newInput = JsonConvert.DeserializeObject<NewInputIntegrationEvent>(message);

            if (newInput == null || string.IsNullOrEmpty(newInput.InputId))
            {
                throw new Exception("Failed to deserialize message!");
            }

            _logger.LogDebug($"Message {newInput.Id} deserialized");

            return newInput;
        }
    }
}
