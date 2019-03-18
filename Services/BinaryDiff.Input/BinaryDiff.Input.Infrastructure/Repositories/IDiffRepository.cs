using BinaryDiff.Input.Domain.Models;
using BinaryDiff.Shared.Infrastructure.MongoDb.Repositories;

namespace BinaryDiff.Input.Infrastructure.Repositories
{
    public interface IDiffRepository : IMongoDbRepository<Diff>
    {
    }
}
