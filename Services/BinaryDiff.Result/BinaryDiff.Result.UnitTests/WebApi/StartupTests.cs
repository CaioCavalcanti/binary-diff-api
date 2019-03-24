using Xunit;

namespace BinaryDiff.Result.UnitTests.WebApi
{
    public class StartupTests
    {
        /// <summary>
        /// Tests if web api subscribes to new result events on event bus
        /// </summary>
        [Fact]
        public void Configure_subscribes_to_new_results_on_event_bus()
        {
            // TODO: implement test

            //eventBus.Verify(eb =>
            //    eb.Subscribe<NewResultIntegrationEvent, NewResultIntegrationEventHandler>(),
            //    Times.Once
            //);
        }

        /// <summary>
        /// Tests if web api is configured to use default exception handler
        /// </summary>
        [Fact]
        public void Configure_uses_default_ExceptionHandlerExceptionMiddleware()
        {
            // TODO: implement test
        }
    }
}
