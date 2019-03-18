using RabbitMQ.Client;
using System;

namespace BinaryDiff.Shared.Infrastructure.RabbitMQ.Connections
{
    public interface IRabbitMQPersistentConnection : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}
