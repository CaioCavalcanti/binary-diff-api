using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using BinaryDiff.Shared.Infrastructure.Configuration;
using BinaryDiff.Shared.Infrastructure.MongoDb.Context;
using BinaryDiff.Shared.Infrastructure.RabbitMQ.Connections;
using BinaryDiff.Shared.Infrastructure.RabbitMQ.EventBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;

namespace BinaryDiff.Shared.WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configure Swagger on Web API using default configuration.
        /// Requires API project to have GenerateDocumentationFile set to true on its *.csproj
        /// in order to generate the doc file from comments (output to *.xml)
        /// </summary>
        /// <typeparam name="T">Startup type to get assembly name</typeparam>
        /// <param name="services">Instance of IServiceCollection</param>
        /// <param name="apiTitle">API title</param>
        /// <param name="version">API version</param>
        /// <returns></returns>
        public static IServiceCollection UseSwagger<T>(
            this IServiceCollection services,
            string apiTitle,
            string version,
            bool useDocsFromXmlComments = true
        )
        {
            return services.AddSwaggerGen(cfg =>
            {
                cfg.SwaggerDoc(version, new Info
                {
                    Title = apiTitle,
                    Version = version,
                    Contact = new Contact
                    {
                        Name = "Caio Cavalcanti",
                        Email = "caiofabiomc@gmail.com",
                        Url = "https://github.com/CaioCavalcanti/binary-diff-api"
                    }
                });

                if (useDocsFromXmlComments)
                {
                    var xmlFile = $"{typeof(T).Assembly.GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                    cfg.IncludeXmlComments(xmlPath);
                }
            });
        }

        /// <summary>
        /// Initialize AutoMapper, getting mappings from classes that inherits Profile,
        /// validating the configuration and providing it on IoC
        /// </summary>
        /// <param name="services">Instance of IServiceCollection</param>
        /// <returns></returns>
        public static IServiceCollection UseAutoMapper(this IServiceCollection services)
        {
            Mapper.Initialize(cfg => { });
            Mapper.AssertConfigurationIsValid();

            return services.AddAutoMapper();
        }

        public static IServiceCollection UseMongoDb(this IServiceCollection services, IConfiguration config)
        {
            var mongoConfig = config.GetSection("mongodb") ?? throw new Exception("Section 'mongodb' not found on host settings");

            return services
                .AddOptions()
                .Configure<MongoConfiguration>(mongoConfig)
                .AddSingleton<IMongoDbContext, MongoDbContext>();
        }

        public static IServiceCollection UseRabbitMQ(this IServiceCollection services, IConfiguration config)
        {
            var rabbitMQConfig = config.GetSection("rabbitmq") ?? throw new Exception("Section 'rabbitmq' not found on host settings");

            return services
                .AddOptions()
                .Configure<RabbitMQConfiguration>(rabbitMQConfig)
                .AddSingleton<IRabbitMQPersistentConnection, RabbitMQPersistentConnection>()
                .AddSingleton<IRabbitMQEventBus, RabbitMQEventBus>();
        }

        public static AutofacServiceProvider UseAutoFacServiceProvider(this IServiceCollection services)
        {
            var container = new ContainerBuilder();

            container.Populate(services);

            return new AutofacServiceProvider(container.Build());
        }
    }
}
