using BinaryDiff.Input.Domain.Models;
using BinaryDiff.Input.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BinaryDiff.Input.Infrastructure.Repositories.Implementation
{
    public class MongoContext : IMongoContext
    {
        public MongoContext(IOptions<MongoConfiguration> configOptions, ILogger<MongoContext> logger)
        {
            logger.LogTrace("Initializing mongo context...");
            var config = configOptions?.Value;

            if (config == null)
            {
                var ex = new MongoConfigurationException("Mongo settings not configured on host");

                logger.LogError(ex, ex.Message);

                throw ex;
            }

            var database = config.GetDatabase();

            logger.LogInformation($"Mongo connection: ${database.Client.Settings}");

            Diffs = database.GetCollection<Diff>("diffs");
            Inputs = database.GetCollection<InputData>("inputs");
        }

        public IMongoCollection<Diff> Diffs { get; }

        public IMongoCollection<InputData> Inputs { get; }
    }
}
