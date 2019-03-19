using BinaryDiff.Result.Domain.Models;
using BinaryDiff.Shared.Infrastructure.RelationalDatabase.Repositories;
using System;
using System.Threading.Tasks;

namespace BinaryDiff.Result.Infrastructure.Repositories
{
    public interface IDiffResultsRepository : IBaseRepository<DiffResult>
    {
        Task<DiffResult> GetLastResultForDiffAsync(Guid diffId);
    }
}
