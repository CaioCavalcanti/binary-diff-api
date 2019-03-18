using BinaryDiff.Shared.Infrastructure.RabbitMQ;
using System;

namespace BinaryDiff.Input.WebApi.IntegrationEvents
{
    public class NewInputIntegrationEvent : IntegrationEvent
    {
        public NewInputIntegrationEvent(Guid diffId, string inputId, DateTime timestamp) : base()
        {
            DiffId = diffId.ToString();
            InputId = inputId;
            Timestamp = timestamp;
        }

        public string InputId { get; set; }

        public string DiffId { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
