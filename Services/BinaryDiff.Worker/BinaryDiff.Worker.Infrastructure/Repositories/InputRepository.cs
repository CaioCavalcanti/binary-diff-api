using BinaryDiff.Shared.Infrastructure.MongoDb.Context;
using BinaryDiff.Shared.Infrastructure.MongoDb.Repositories;
using BinaryDiff.Worker.Domain.Enums;
using BinaryDiff.Worker.Domain.Models;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace BinaryDiff.Worker.Infrastructure.Repositories
{
    public class InputRepository : MongoDbReadOnlyRepository<InputData>, IInputRepository
    {
        const string COLLECTION_NAME = "diffInputs";

        public InputRepository(IMongoDbContext mongoDbContext)
            : base(mongoDbContext, COLLECTION_NAME)
        {
        }

        public Task<InputData> GetLastInputBeforeAsync(Guid diffId, InputPosition position, DateTime timestamp)
        {
            return collection
                .Find(input =>
                    input.DiffId == diffId &&
                    input.Position == position &&
                    input.Timestamp <= timestamp
                )
                .SortByDescending(input => input.Timestamp)
                .FirstOrDefaultAsync();
        }
    }
}
