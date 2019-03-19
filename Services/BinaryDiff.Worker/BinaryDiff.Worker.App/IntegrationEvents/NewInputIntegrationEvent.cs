using BinaryDiff.Shared.Infrastructure.RabbitMQ;
using BinaryDiff.Worker.Domain.Enums;
using System;

namespace BinaryDiff.Worker.App.IntegrationEvents
{
    public class NewInputIntegrationEvent : IntegrationEvent
    {
        public string InputId { get; set; }

        public InputPosition Position { get; set; }

        public Guid DiffId { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
