﻿using BinaryDiff.Shared.Infrastructure.RabbitMQ.EventBus;

namespace BinaryDiff.Worker.Infrastructure.EventBus
{
    public interface IInputEventBus : IRabbitMQEventBus
    {
    }
}
