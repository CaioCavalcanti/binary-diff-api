using BinaryDiff.Input.Infrastructure.EventBus;
using BinaryDiff.Input.Infrastructure.EventBus.Implementation;
using BinaryDiff.Input.Infrastructure.Repositories;
using BinaryDiff.Input.Infrastructure.Repositories.Implementation;
using BinaryDiff.Shared.WebApi.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BinaryDiff.Input.WebApi
{
    public class Startup
    {
        const string API_NAME = "BinaryDiff.Input Web API";
        const string API_VERSION = "v1";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .ConfigureJsonSerializerSettings();

            services
                .ConfigureSwagger<Startup>(API_NAME, API_VERSION)
                .UseMongoDb(Configuration)
                    .AddSingleton<IDiffRepository, DiffRepository>()
                    .AddSingleton<IInputRepository, InputRepository>()
                .UseRabbitMQ(Configuration)
                    .AddSingleton<IInputEventBus, InputEventBus>()
                .UseAutoMapper();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseDefaultExceptionHandler()
                .UseSwagger(API_NAME, API_VERSION)
                .UseMvc();
        }
    }
}
