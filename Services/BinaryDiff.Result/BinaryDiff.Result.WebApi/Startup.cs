using BinaryDiff.Result.Infrastructure.Database;
using BinaryDiff.Result.Infrastructure.Repositories;
using BinaryDiff.Shared.WebApi.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .ConfigureJsonSerializerSettings();

            services
                .ConfigureSwagger<Startup>(API_NAME, API_VERSION)
                .UseAutoMapper();

            ConfigureDatabase(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            RunMigrations(app);

            app.UseDefaultExceptionHandler()
               .UseSwagger(API_NAME, API_VERSION)
               .UseMvc();
        }

        private void ConfigureDatabase(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("postgres");

            services
                .AddEntityFrameworkNpgsql()
                .AddDbContext<ResultContext>(
                    options => options.UseNpgsql(connectionString)
                )
                .AddScoped<IUnitOfWork, UnitOfWork>(); ;
        }

        private void RunMigrations(IApplicationBuilder app)
        {
            using (var theServiceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var resultDbContext = theServiceScope.ServiceProvider.GetService<ResultContext>();
                resultDbContext.Database.Migrate();
            }
        }
    }
}
