using System;

namespace BinaryDiff.IntegrationTests
{
    public class ApiGatewayRoutes
    {
        public static class Get
        {
            public static string DiffResult(Guid diffId) => $"/v1/diff/{diffId}";
        }

        public static class Post
        {
            public static string CreateDiff = "/v1/diff";

            public static string AddInputToLeft(Guid diffId) => $"/v1/diff/{diffId}/left";

            public static string AddInputToRight(Guid diffId) => $"/v1/diff/{diffId}/right";
        }
    }
}
