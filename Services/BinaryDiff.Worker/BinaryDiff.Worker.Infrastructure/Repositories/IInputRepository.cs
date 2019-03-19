using BinaryDiff.Shared.Infrastructure.MongoDb.Repositories;
using BinaryDiff.Worker.Domain.Enums;
using BinaryDiff.Worker.Domain.Models;
using System;
using System.Threading.Tasks;

namespace BinaryDiff.Worker.Infrastructure.Repositories
{
    public interface IInputRepository : IMongoDbReadOnlyRepository<InputData>
    {
        Task<InputData> GetLastInputBeforeAsync(Guid diffId, InputPosition position, DateTime timestamp);
    }
}
