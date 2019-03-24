namespace BinaryDiff.Shared.Infrastructure.Configuration
{
    public class MongoDbConfiguration
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string Database { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public string UserDatabase { get; set; }
    }
}
