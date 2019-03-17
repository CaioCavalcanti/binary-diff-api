using MongoDB.Bson;
using MongoDB.Driver;

namespace BinaryDiff.Input.Infrastructure.Configuration
{
    public static class MongoConfigurationExtensions
    {
        public static IMongoDatabase GetDatabase(this MongoConfiguration mongoConfig)
        {
            BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;

            var credentials = MongoCredential.CreateCredential(mongoConfig.UserDatabase, mongoConfig.User, mongoConfig.Password);

            var clientSettings = new MongoClientSettings
            {
                Credential = credentials,
                Server = new MongoServerAddress(mongoConfig.Host, mongoConfig.Port)
            };

            var client = new MongoClient(clientSettings);

            return client.GetDatabase(mongoConfig.Database);
        }
    }
}
