using BinaryDiff.API.Helpers.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BinaryDiff.API.Helpers
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("The response has already started, the exception handler middleware will not be executed.");
                    throw;
                }

                var errorId = Guid.NewGuid();
                var message = new ExceptionMessage(errorId);

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(message));

                _logger.LogError(ex, $"[ErrorId={errorId.ToString()}] {context.Request.Method} {context.Request.Path}");

                return;
            }
        }
    }
}
