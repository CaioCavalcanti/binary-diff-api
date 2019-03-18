using BinaryDiff.Input.Domain.Models;
using BinaryDiff.Shared.Infrastructure.MongoDb.Context;
using BinaryDiff.Shared.Infrastructure.MongoDb.Repositories;

namespace BinaryDiff.Input.Infrastructure.Repositories.Implementation
{
    public class InputRepository : MongoDbRepository<InputData>, IInputRepository
    {
        const string COLLECTION_NAME = "diffInputs";

        public InputRepository(IMongoDbContext mongoDbContext)
            : base(mongoDbContext, COLLECTION_NAME)
        {
        }
    }
}
