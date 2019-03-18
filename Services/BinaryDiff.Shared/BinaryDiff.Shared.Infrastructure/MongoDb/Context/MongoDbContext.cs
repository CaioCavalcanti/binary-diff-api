using BinaryDiff.Shared.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BinaryDiff.Shared.Infrastructure.MongoDb.Context
{
    public class MongoDbContext : IMongoDbContext
    {
        public MongoDbContext(IOptions<MongoConfiguration> configOptions, ILogger<MongoDbContext> logger)
        {
            Database = MongoDbFactory.GetDatabase(configOptions?.Value);
        }

        public IMongoDatabase Database { get; }
    }
}
