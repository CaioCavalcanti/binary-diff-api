using BinaryDiff.Input.Domain.Enums;
using BinaryDiff.Input.Domain.Models;
using BinaryDiff.Shared.Infrastructure.RabbitMQ.Events;
using System;

namespace BinaryDiff.Input.WebApi.IntegrationEvents
{
    public class NewInputIntegrationEvent : IntegrationEvent
    {
        public NewInputIntegrationEvent(InputData input) : base()
        {
            DiffId = input.DiffId.ToString(); ;
            InputId = input.Id;
            Position = input.Position;
            Timestamp = input.Timestamp;
        }

        public string InputId { get; set; }

        public string DiffId { get; set; }

        public InputPosition Position { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
