using BinaryDiff.Result.Domain.Models;
using System;
using System.Threading.Tasks;

namespace BinaryDiff.Result.Infrastructure.Repositories
{
    public interface IDiffResultRepository : IBaseRepository<DiffResult>
    {
        Task<DiffResult> GetLastResultForDiffAsync(Guid diffId);
    }
}
