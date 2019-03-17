using BinaryDiff.Input.Domain.Models;

namespace BinaryDiff.Input.Infrastructure.Repositories.Implementation
{
    public class DiffRepository : DocumentRepository<Diff>, IDiffRepository
    {
        public DiffRepository(IMongoContext context) : base(context.Diffs)
        {
        }
    }
}
