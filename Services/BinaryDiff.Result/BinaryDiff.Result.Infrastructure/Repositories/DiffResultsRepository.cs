using BinaryDiff.Result.Domain.Models;
using BinaryDiff.Result.Infrastructure.Database;
using BinaryDiff.Shared.Infrastructure.RelationalDatabase.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BinaryDiff.Result.Infrastructure.Repositories.Implementation
{
    public class DiffResultsRepository : BaseRepository<DiffResult>, IDiffResultsRepository
    {
        public DiffResultsRepository(ResultContext context) : base(context) { }

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
