using MongoDB.Driver;

namespace BinaryDiff.Shared.Infrastructure.MongoDb.Context
{
    public interface IMongoDbContext
    {
        IMongoDatabase Database { get; }
    }
}
