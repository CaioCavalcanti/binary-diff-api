using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using BinaryDiff.Shared.Infrastructure.MongoDb.Context;
using BinaryDiff.Shared.Infrastructure.RabbitMQ.Connections;
using BinaryDiff.Shared.Infrastructure.RabbitMQ.EventBus;
using BinaryDiff.Shared.WebApi.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Xunit;

namespace BinaryDiff.Shared.UnitTests.WebApi
{
    public class ServiceCollectionExtensionsTests
    {
        /// <summary>
        /// Tests if UseMongoDb injects IMongoDbContext on container
        /// </summary>
        [Fact]
        public void UseMongoDb_adds_IMongoDbContext_to_container_using_host_configuration()
        {
            // Arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(_mongoConfig)
                .Build();

            var services = new ServiceCollection()
                .AddLogging();

            // Act
            services.UseMongoDb(config);

            // Assert
            using (var sp = services.BuildServiceProvider())
            {
                Assert.NotNull(sp.GetRequiredService<IMongoDbContext>());
            }
        }

        /// <summary>
        /// Tests if UseMongoDb throws exception in case 'mongodb' section is not set on app configuration
        /// </summary>
        [Fact]
        public void UseMongoDb_throws_exception_if_mongodb_section_not_found_on_configuration()
        {
            // Arrange
            var config = new ConfigurationBuilder()
                .Build();

            var services = new ServiceCollection();

            // Act
            services.AddLogging();
            services.UseMongoDb(config);

            // Assert
            using (var sp = services.BuildServiceProvider())
            {
                Assert.Throws<ArgumentNullException>(() => sp.GetRequiredService<IMongoDbContext>());
            }
        }

        /// <summary>
        /// Tests if UseRabbitMQ injects IRabbitMQPersistentConnection and IRabbitMQEventBus on container
        /// </summary>
        [Fact]
        public void UseRabbitMQ_adds_IRabbitMQEventBus_to_container_using_host_configuration()
        {
            // Arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(_rabbitMQConfig)
                .Build();

            var services = new ServiceCollection()
                .AddLogging();

            // Act
            services.UseRabbitMQ(config);

            var container = new ContainerBuilder();
            container.Populate(services);

            // Assert
            using (var sp = new AutofacServiceProvider(container.Build()))
            {
                Assert.NotNull(sp.GetRequiredService<IRabbitMQPersistentConnection>());
                Assert.NotNull(sp.GetRequiredService<IRabbitMQEventBus>());
            }
        }

        /// <summary>
        /// Tests if UseMongoDb throws exception in case 'rabbitmq' section is not set on app configuration
        /// </summary>
        [Fact]
        public void UseRabbitMQ_throws_exception_if_rabbitmq_section_not_found_on_configuration()
        {
            // Arrange
            var config = new ConfigurationBuilder()
                .Build();

            var services = new ServiceCollection()
                .AddLogging();

            // Act
            services.UseRabbitMQ(config);

            var container = new ContainerBuilder();
            container.Populate(services);

            // Assert
            using (var sp = new AutofacServiceProvider(container.Build()))
            {
                Assert.Throws<DependencyResolutionException>(() => sp.GetRequiredService<IRabbitMQPersistentConnection>());
            }
        }

        /// <summary>
        /// Tests if UseAutoMapper initializes mappings and injects IMapper on container
        /// </summary>
        [Fact]
        public void UseAutoMapper_initializes_autoMapper_and_injects_IMapper_on_container()
        {
            // Arrange
            var services = new ServiceCollection()
                .AddLogging();

            // Act
            services.UseAutoMapper();

            // Arrange
            using (var sp = services.BuildServiceProvider())
            {
                Assert.NotNull(sp.GetRequiredService<IMapper>());
                // shouldn't let you initialize twice
                Assert.Throws<InvalidOperationException>(() => Mapper.Initialize(cfg => { }));
            }
        }

        private readonly KeyValuePair<string, string>[] _mongoConfig = new KeyValuePair<string, string>[] {
                new KeyValuePair<string, string>("mongodb:host", "aHost"),
                new KeyValuePair<string, string>("mongodb:port", "27272"),
                new KeyValuePair<string, string>("mongodb:database", "aDatabase"),
                new KeyValuePair<string, string>("mongodb:user", "anUser"),
                new KeyValuePair<string, string>("mongodb:password", "aPassword"),
                new KeyValuePair<string, string>("mongodb:userDatabase", "anUserDatabase"),
            };

        private readonly KeyValuePair<string, string>[] _rabbitMQConfig = new KeyValuePair<string, string>[] {
                new KeyValuePair<string, string>("rabbitmq:host", "aHost"),
                new KeyValuePair<string, string>("rabbitmq:port", "5656"),
                new KeyValuePair<string, string>("rabbitmq:user", "anUser"),
                new KeyValuePair<string, string>("rabbitmq:password", "aPassword"),
                new KeyValuePair<string, string>("rabbitmq:retryCount", "5"),
            };
    }
}
