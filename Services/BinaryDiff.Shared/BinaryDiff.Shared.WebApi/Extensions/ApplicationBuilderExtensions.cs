using BinaryDiff.Shared.WebApi.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace BinaryDiff.Shared.WebApi.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, string title, string version)
        {
            return app
                .UseSwagger()
                .UseSwaggerUI(cfg =>
                {
                    cfg.SwaggerEndpoint($"/swagger/{version}/swagger.json", title);
                    cfg.RoutePrefix = string.Empty;
                });
        }

        public static IApplicationBuilder UseDefaultExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}
