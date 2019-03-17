using BinaryDiff.Input.Domain.Models;
using MongoDB.Driver;

namespace BinaryDiff.Input.Infrastructure.Repositories
{
    public interface IMongoContext
    {
        IMongoCollection<Diff> Diffs { get; }

        IMongoCollection<InputData> Inputs { get; }
    }
}
