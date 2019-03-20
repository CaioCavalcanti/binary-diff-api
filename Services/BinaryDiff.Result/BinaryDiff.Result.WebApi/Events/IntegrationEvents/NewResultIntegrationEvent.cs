using BinaryDiff.Result.Domain.Enums;
using BinaryDiff.Shared.Infrastructure.RabbitMQ.Events;
using System;
using System.Collections.Generic;

namespace BinaryDiff.Result.WebApi.Events.IntegrationEvents
{
    public class NewResultIntegrationEvent : IntegrationEvent
    {
        public NewResultIntegrationEvent() : base() { }

        public Guid DiffId { get; set; }

        public InputPosition InputPosition { get; set; }

        public string InputId { get; set; }

        public string OpposingId { get; set; }

        public ResultType Result { get; set; }

        public IDictionary<int, int> Differences { get; set; }
    }
}
