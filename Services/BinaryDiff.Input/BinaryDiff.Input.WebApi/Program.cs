using BinaryDiff.Shared.WebApi.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace BinaryDiff.Input.WebApi
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
                    config.UseDefaultConfiguration(hostingContext.HostingEnvironment)
                )
                .ConfigureLogging((hostingContext, logging) => 
                    logging.UseDefault()
                )
                .UseStartup<Startup>();
    }
}
