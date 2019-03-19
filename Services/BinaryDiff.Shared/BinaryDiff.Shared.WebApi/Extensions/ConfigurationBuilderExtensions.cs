using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace BinaryDiff.Shared.WebApi.Extensions
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder UseDefaultConfiguration(this IConfigurationBuilder builder, IHostingEnvironment environment)
        {
            return builder.UseDefaultConfiguration(environment.ContentRootPath, environment.EnvironmentName);
        }

        public static IConfigurationBuilder UseDefaultConfiguration(this IConfigurationBuilder builder, string basePath, string environment)
        {
            return builder
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{environment}.json", true, true)
                .AddEnvironmentVariables();
        }
    }
}
