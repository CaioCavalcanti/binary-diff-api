using BinaryDiff.Domain.Logic;
using BinaryDiff.Domain.Logic.Implementation;
using BinaryDiff.Domain.Models;
using BinaryDiff.Infrastructure.Repositories;
using BinaryDiff.Infrastructure.Repositories.Implementation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Reflection;

namespace BinaryDiff.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .AddJsonOptions(opt =>
                {
                    opt.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                    opt.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            ConfigureIoC(services);
            ConfigureSwagger(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            ConfigureSwagger(app);

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private void ConfigureIoC(IServiceCollection services)
        {
            services
                .AddSingleton<IMemoryRepository<Guid, DiffModel>, MemoryRepository<Guid, DiffModel>>()
                .AddSingleton<ILeftRepository, LeftRepository>()
                .AddSingleton<IRightRepository, RightRepository>()
                .AddTransient<IDiffLogic, DiffLogic>();
        }

        private void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(cfg =>
            {
                cfg.SwaggerDoc("v1", new Info
                {
                    Title = "Binary Diff API v1",
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
                cfg.SwaggerEndpoint("/swagger/v1/swagger.json", "Binary Diff API v1");
                cfg.RoutePrefix = string.Empty;
            });
        }
    }
}
