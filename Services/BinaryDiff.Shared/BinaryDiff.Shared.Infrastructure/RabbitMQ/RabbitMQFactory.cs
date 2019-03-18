using BinaryDiff.Shared.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Net.Sockets;

namespace BinaryDiff.Shared.Infrastructure.RabbitMQ
{
    public static class RabbitMQFactory
    {
        public static ConnectionFactory GetConnectionFactory(RabbitMQConfiguration rabbitMQConfig)
        {
            if (rabbitMQConfig == null) throw new ArgumentNullException(nameof(rabbitMQConfig));

            return new ConnectionFactory()
            {
                HostName = rabbitMQConfig.Host,
                Port = rabbitMQConfig.Port,
                UserName = rabbitMQConfig.User,
                Password = rabbitMQConfig.Password
            };
        }

        public static RetryPolicy GetDefaultRetryPolicy(ILogger logger, int retryCount)
        {
            return RetryPolicy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(
                    retryCount,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (ex, time) => logger.LogWarning(ex.ToString()));
        }
    }
}
