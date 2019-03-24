using BinaryDiff.Shared.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;

namespace BinaryDiff.Shared.Infrastructure.MongoDb.Context
{
    public class MongoDbContext : IMongoDbContext
    {
        private readonly IMongoClient _client;

        public MongoDbContext(IOptions<MongoDbConfiguration> configOptions, ILogger<MongoDbContext> logger)
        {
            var config = configOptions?.Value ?? throw new ArgumentNullException(nameof(configOptions));

            _client = MongoDbFactory.GetClient(config);
            Database = _client.GetDatabase(config.Database);
        }

        public IMongoDatabase Database { get; }
    }
}
