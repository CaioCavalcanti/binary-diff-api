using BinaryDiff.Shared.Infrastructure.RabbitMQ.Events;
using BinaryDiff.Worker.Domain.Enums;
using System;

namespace BinaryDiff.Worker.App.Events.IntegrationEvents
{
    public class NewInputIntegrationEvent : IntegrationEvent
    {
        public string InputId { get; set; }

        public InputPosition Position { get; set; }

        public Guid DiffId { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
