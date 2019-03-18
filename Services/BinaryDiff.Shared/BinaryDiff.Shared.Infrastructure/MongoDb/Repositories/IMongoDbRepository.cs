using BinaryDiff.Shared.Domain.Models;
using System.Threading.Tasks;

namespace BinaryDiff.Shared.Infrastructure.MongoDb.Repositories
{
    public interface IMongoDbRepository<TDocument> : IMongoDbReadOnlyRepository<TDocument>
        where TDocument : BaseDocument
    {
        Task AddOneAsync(TDocument document);
    }
}
