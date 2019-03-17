using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace BinaryDiff.Input.Infrastructure.Helpers
{
    public static class ConfigurationExtensions
    {
        public static IMongoDatabase GetMongoDatabase(this IConfiguration config)
        {
            BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;

            var credentials = MongoCredential.CreateCredential(
                config["mongo:masterDatabase"],
                config["mongo:username"],
                config["mongo:password"]);

            var clientSettings = new MongoClientSettings
            {
                Credential = credentials,
                Server = new MongoServerAddress(config["mongo:host"], Convert.ToInt32(config["mongo:port"]))
            };

            var client = new MongoClient(clientSettings);

            return client.GetDatabase(config["mongo:targetDatabase"]);
        }
    }
}
