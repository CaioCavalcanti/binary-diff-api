using BinaryDiff.Shared.Infrastructure.RabbitMQ;
using BinaryDiff.Worker.Domain.Enums;
using BinaryDiff.Worker.Domain.Models;
using System;
using System.Collections.Generic;

namespace BinaryDiff.Worker.App.IntegrationEvents
{
    public class NewResultIntegrationEvent : IntegrationEvent
    {
        public NewResultIntegrationEvent(InputData input, string opposingId, DiffResult diffResult) : base()
        {
            DiffId = input.DiffId;
            InputPosition = input.Position;
            InputId = input.Id;
            OpposingId = opposingId;
            Result = diffResult.Result;
            Differences = diffResult.Differences;
        }

        public Guid DiffId { get; set; }

        public InputPosition InputPosition { get; set; }

        public string InputId { get; set; }

        public string OpposingId { get; set; }

        public ResultType Result { get; set; }

        public IDictionary<int, int> Differences { get; set; }
    }
}
