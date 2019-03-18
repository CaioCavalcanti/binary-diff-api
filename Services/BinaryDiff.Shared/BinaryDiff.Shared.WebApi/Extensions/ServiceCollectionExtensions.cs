using AutoMapper;
using BinaryDiff.Shared.Infrastructure.Configuration;
using BinaryDiff.Shared.Infrastructure.MongoDb.Context;
using BinaryDiff.Shared.Infrastructure.RabbitMQ.Connections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;

namespace BinaryDiff.Shared.WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureSwagger<T>(this IServiceCollection services, string apiTitle, string version)
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

                var xmlFile = $"{typeof(T).Assembly.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                cfg.IncludeXmlComments(xmlPath);
            });
        }

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
                .AddSingleton<IRabbitMQPersistentConnection, RabbitMQPersistentConnection>();
        }
    }
}
