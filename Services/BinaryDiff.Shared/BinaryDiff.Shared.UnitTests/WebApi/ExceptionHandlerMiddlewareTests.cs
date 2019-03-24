using BinaryDiff.Shared.WebApi.Middlewares;
using BinaryDiff.Shared.WebApi.ResultMessages;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace BinaryDiff.Shared.UnitTests.WebApi
{
    public class ExceptionHandlerMiddlewareTests
    {
        /// <summary>
        /// Tests if exception handler logs and returns an error id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ExceptionHandler_logs_and_returns_errorId()
        {
            // Arrange
            var expectedException = new Exception("Any exception...");
            var logger = new Mock<ILogger<ExceptionHandlerMiddleware>>();
            var exceptionHandler = new ExceptionHandlerMiddleware(
                (innerHttpContext) => { throw expectedException; },
                logger.Object
            );

            var context = GetContext();

            // Act
            await exceptionHandler.Invoke(context);

            var resultMessage = GetResultMessage(context);

            // Assert
            Assert.NotEqual(Guid.Empty, resultMessage.ErrorId);
            logger.Verify(l =>
                l.Log(
                    LogLevel.Error,
                    0,
                    It.Is<FormattedLogValues>(v => v.ToString() == $"[ErrorId={resultMessage.ErrorId}] {context.Request.Method} {context.Request.Path}"),
                    It.Is<Exception>(e => e == expectedException),
                    It.IsAny<Func<object, Exception, string>>())
            );
        }

        /// <summary>
        /// Tests if exception handler returns exception result message on exception
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ExceptionHandler_returns_ExceptionResultMessage()
        {
            // Arrange
            var expectedException = new Exception("Any exception...");
            var logger = new Mock<ILogger<ExceptionHandlerMiddleware>>();
            var exceptionHandler = new ExceptionHandlerMiddleware(
                (innerHttpContext) => { throw expectedException; },
                logger.Object
            );

            var context = GetContext();

            // Act
            await exceptionHandler.Invoke(context);

            var resultMessage = GetResultMessage(context);

            // Assert
            Assert.NotNull(resultMessage);
            Assert.NotEqual(Guid.Empty, resultMessage.ErrorId);
            Assert.Equal(
                "An error occurred and your message couldn't be processed. If the error persists, contact the admin informing the error id provided.",
                resultMessage.Message
            );
        }

        private DefaultHttpContext GetContext()
        {
            var context = new DefaultHttpContext();
            context.Request.Method = "POST";
            context.Request.Path = new PathString("/");
            context.Response.Body = new MemoryStream();

            return context;
        }

        private ExceptionResultMessage GetResultMessage(DefaultHttpContext context)
        {
            ExceptionResultMessage message;
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            using (var reader = new StreamReader(context.Response.Body))
            {
                var streamText = reader.ReadToEnd();
                message = JsonConvert.DeserializeObject<ExceptionResultMessage>(streamText);
            }

            return message;

        }
    }
}
