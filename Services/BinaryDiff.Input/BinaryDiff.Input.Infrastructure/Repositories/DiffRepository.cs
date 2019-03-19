using BinaryDiff.Input.Domain.Models;
using BinaryDiff.Shared.Infrastructure.MongoDb.Context;
using BinaryDiff.Shared.Infrastructure.MongoDb.Repositories;

namespace BinaryDiff.Input.Infrastructure.Repositories
{
    public class DiffRepository : MongoDbRepository<Diff>, IDiffRepository
    {
        const string COLLECTION_NAME = "diffs";

        public DiffRepository(IMongoDbContext mongoDbContext)
            : base(mongoDbContext, COLLECTION_NAME)
        {
        }
    }
}
