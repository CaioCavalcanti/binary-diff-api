using BinaryDiff.Result.Domain.Models;
using BinaryDiff.Shared.Infrastructure.RelationalDatabase.Repositories;

namespace BinaryDiff.Result.Infrastructure.Repositories
{
    public interface IInputDifferencesRepository : IBaseRepository<InputDifference>
    {
    }
}
