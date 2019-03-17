using AutoMapper;
using BinaryDiff.Result.Infrastructure.Database;
using BinaryDiff.Result.Infrastructure.Repositories;
using BinaryDiff.Result.Infrastructure.Repositories.Implementation;
using BinaryDiff.Result.WebApi.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Reflection;

namespace BinaryDiff.Result.WebApi
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

            ConfigureDatabase(services);
            ConfigureSwagger(services);
            ConfigureAutoMapper(services);
            ConfigureIoC(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            ConfigureSwagger(app);

            app.UseMiddleware<ExceptionHandlerMiddleware>();

            app.UseMvc();
        }

        private void ConfigureIoC(IServiceCollection services)
        {
            services
                .AddScoped<IUnitOfWork, UnitOfWork>()
                .AddScoped<IDiffResultRepository, DiffResultRepository>();
        }

        private void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(cfg =>
            {
                cfg.SwaggerDoc("v1", new Info
                {
                    Title = "BinaryDiff.Result Web API v1",
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
                cfg.SwaggerEndpoint("/swagger/v1/swagger.json", "BinaryDiff.Result Web API v1");
                cfg.RoutePrefix = string.Empty;
            });
        }

        private void ConfigureAutoMapper(IServiceCollection services)
        {
            Mapper.Initialize(cfg => { });

            services.AddAutoMapper();

            Mapper.AssertConfigurationIsValid();
        }

        private void ConfigureDatabase(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("postgres");

            services
                .AddEntityFrameworkNpgsql()
                .AddDbContext<ResultContext>(
                    options => options.UseNpgsql(connectionString)
                );
        }
    }
}
