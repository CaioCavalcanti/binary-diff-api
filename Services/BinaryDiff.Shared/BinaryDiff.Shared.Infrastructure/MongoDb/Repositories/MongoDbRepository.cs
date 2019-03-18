using BinaryDiff.Shared.Domain.Models;
using BinaryDiff.Shared.Infrastructure.MongoDb.Context;
using System.Threading.Tasks;

namespace BinaryDiff.Shared.Infrastructure.MongoDb.Repositories
{
    public class MongoDbRepository<TDocument> : MongoDbReadOnlyRepository<TDocument>, IMongoDbRepository<TDocument>
        where TDocument : BaseDocument
    {
        public MongoDbRepository(IMongoDbContext mongoDbContext, string collectionName)
            : base(mongoDbContext, collectionName)
        {
        }

        public async Task AddOneAsync(TDocument document)
        {
            await collection.InsertOneAsync(document);
        }
    }
}
