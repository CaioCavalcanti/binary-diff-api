using BinaryDiff.Result.Infrastructure.Database;
using BinaryDiff.Result.Infrastructure.Repositories;
using BinaryDiff.Result.WebApi.Events.IntegrationEventHandlers;
using BinaryDiff.Result.WebApi.Events.IntegrationEvents;
using BinaryDiff.Shared.Infrastructure.RabbitMQ.EventBus;
using BinaryDiff.Shared.WebApi.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BinaryDiff.Result.WebApi
{
    public class Startup
    {
        const string API_NAME = "BinaryDiff.Result Web API";
        const string API_VERSION = "v1";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .ConfigureJsonSerializerSettings();

            ConfigureDatabase(services);

            return services
                .UseSwagger<Startup>(API_NAME, API_VERSION)
                .UseAutoMapper()
                .UseRabbitMQ(Configuration)
                    .AddTransient<NewResultIntegrationEventHandler>()
                .UseAutoFacServiceProvider();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            RunMigrations(app);
            ConfigureEventBus(app);

            app.UseDefaultExceptionHandler()
               .UseSwagger(API_NAME, API_VERSION)
               .UseMvc();
        }

        /// <summary>
        /// Configure Postgres database
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureDatabase(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("postgres");

            services
                .AddEntityFrameworkNpgsql()
                .AddDbContext<ResultContext>(
                    options => options.UseNpgsql(connectionString)
                )
                .AddScoped<IUnitOfWork, UnitOfWork>();
        }

        /// <summary>
        /// Execute ef migrations
        /// </summary>
        /// <param name="app"></param>
        private void RunMigrations(IApplicationBuilder app)
        {
            using (var theServiceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var resultDbContext = theServiceScope.ServiceProvider.GetService<ResultContext>();
                resultDbContext.Database.Migrate();
            }
        }

        /// <summary>
        /// Configure event bus subscriptions
        /// </summary>
        /// <param name="app"></param>
        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IRabbitMQEventBus>();

            eventBus.Subscribe<NewResultIntegrationEvent, NewResultIntegrationEventHandler>();
        }
    }
}
