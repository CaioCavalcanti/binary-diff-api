using AutoMapper;
using BinaryDiff.Input.Infrastructure.Configuration;
using BinaryDiff.Input.Infrastructure.Repositories;
using BinaryDiff.Input.Infrastructure.Repositories.Implementation;
using BinaryDiff.Input.WebApi.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Reflection;

namespace BinaryDiff.Input.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .AddJsonOptions(opt =>
                {
                    opt.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                    opt.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
                });

            ConfigureOptions(services);
            ConfigureIoC(services);
            ConfigureSwagger(services);
            ConfigureAutoMapper(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            ConfigureSwagger(app);

            app.UseMiddleware<ExceptionHandlerMiddleware>();

            app.UseMvc();
        }

        private void ConfigureOptions(IServiceCollection services)
        {
            services.AddOptions();

            services.Configure<MongoConfiguration>(Configuration.GetSection("mongo"));
        }

        private void ConfigureIoC(IServiceCollection services)
        {
            services
                .AddSingleton<IMongoContext, MongoContext>()
                .AddSingleton<IDiffRepository, DiffRepository>()
                .AddSingleton<IInputRepository, InputRepository>();
        }

        private void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(cfg =>
            {
                cfg.SwaggerDoc("v1", new Info
                {
                    Title = "BinaryDiff.Input Web API v1",
                    Version = "v1",
                    Contact = new Contact
                    {
                        Name = "Caio Cavalcanti",
                        Email = "caiofabiomc@gmail.com",
                        Url = "https://github.com/CaioCavalcanti/binary-diff-api"
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                cfg.IncludeXmlComments(xmlPath);
            });
        }

        private void ConfigureSwagger(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(cfg =>
            {
                cfg.SwaggerEndpoint("/swagger/v1/swagger.json", "BinaryDiff.Input Web API v1");
                cfg.RoutePrefix = string.Empty;
            });
        }

        private void ConfigureAutoMapper(IServiceCollection services)
        {
            Mapper.Initialize(cfg => { });

            services.AddAutoMapper();

            Mapper.AssertConfigurationIsValid();
        }
    }
}
