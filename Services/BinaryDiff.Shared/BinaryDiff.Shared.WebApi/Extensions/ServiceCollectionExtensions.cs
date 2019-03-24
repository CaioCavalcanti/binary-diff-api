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
        /// validating the configuration and providing it on container
        /// </summary>
        /// <param name="services">Instance of IServiceCollection</param>
        /// <returns></returns>
        public static IServiceCollection UseAutoMapper(this IServiceCollection services)
        {
            try
            {
                Mapper.Initialize(cfg => { });
            }
            catch (InvalidOperationException ex)
            {
            }
            finally
            {
                Mapper.AssertConfigurationIsValid();
            }


            return services.AddAutoMapper();
        }

        /// <summary>
        /// Adds mongodb configuration options from environment and IMongoDbContext to container as singleton.
        /// Requires 'mongodb' section configured on host (appsettings or environment)
        /// </summary>
        /// <param name="services">Instance of IServiceCollection</param>
        /// <param name="config">App configuration to get section 'mongodb'</param>
        /// <returns></returns>
        public static IServiceCollection UseMongoDb(this IServiceCollection services, IConfiguration config)
        {
            return services
                .AddOptions()
                .Configure<MongoDbConfiguration>(config.GetSection("mongodb"))
                .AddSingleton<IMongoDbContext, MongoDbContext>();
        }

        /// <summary>
        /// Adds rabbitmq configration options form environment and IRabbitMQEventBus to container as singleton
        /// Requires 'rabbitmq' section configured on host (appsettings or environment).
        /// </summary>
        /// <param name="services">Instance of IServiceCollection</param>
        /// <param name="config">App configuration to get section 'rabbitmq'</param>
        /// <returns>Instance of IServiceCollection with RabbitMQ added on it</returns>
        public static IServiceCollection UseRabbitMQ(this IServiceCollection services, IConfiguration config)
        {
            return services
                .AddOptions()
                .Configure<RabbitMQConfiguration>(config.GetSection("rabbitmq"))
                .AddSingleton<IRabbitMQPersistentConnection, RabbitMQPersistentConnection>()
                .AddSingleton<IRabbitMQEventBus, RabbitMQEventBus>();
        }

        /// <summary>
        /// Generates an AutoFac service provider for a given IServiceCollection.
        /// In this solution this is required on apps that use IRabbitMQEventBus, 
        /// so it can create a ILifeTimeScope to resolve integration event handler 
        /// when a new event is received and dispose it right after executing the handler.
        /// </summary>
        /// <param name="services">Instance of IServiceCollection</param>
        /// <returns>A new AutoFacServiceProvider instance</returns>
        public static AutofacServiceProvider UseAutoFacServiceProvider(this IServiceCollection services)
        {
            var container = new ContainerBuilder();

            container.Populate(services);

            return new AutofacServiceProvider(container.Build());
        }
    }
}
