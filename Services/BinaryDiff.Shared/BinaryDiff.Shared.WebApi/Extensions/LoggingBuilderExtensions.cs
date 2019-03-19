using Microsoft.Extensions.Logging;

namespace BinaryDiff.Shared.WebApi.Extensions
{
    public static class LoggingBuilderExtensions
    {
        public static ILoggingBuilder UseDefault(this ILoggingBuilder logging)
        {
            return logging
                .AddConsole()
                .AddDebug();
        }
    }
}
