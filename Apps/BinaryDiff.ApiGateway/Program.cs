using BinaryDiff.Shared.WebApi.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace BinaryDiff.ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config
                        .UseDefaultConfiguration(hostingContext.HostingEnvironment)
                        .AddJsonFile("ocelot.json");
                })
                .ConfigureLogging((hostingContext, logging) =>
                    logging.UseDefault()
                )
                .UseStartup<Startup>();
    }
}
