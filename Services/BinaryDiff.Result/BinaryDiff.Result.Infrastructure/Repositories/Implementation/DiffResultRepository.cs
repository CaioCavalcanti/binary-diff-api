using BinaryDiff.Result.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BinaryDiff.Result.Infrastructure.Repositories.Implementation
{
    public class DiffResultRepository : BaseRepository<DiffResult>, IDiffResultRepository
    {
        public DiffResultRepository(IUnitOfWork uow) : base(uow) { }

        public Task<DiffResult> GetLastResultForDiffAsync(Guid diffId)
        {
            return dbSet
                .Where(r => r.DiffId == diffId)
                .OrderBy(r => r.Timestamp)
                .Include(r => r.Differences)
                .LastOrDefaultAsync();
        }
    }
}
