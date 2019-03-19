using BinaryDiff.Result.Domain.Models;
using BinaryDiff.Result.Infrastructure.Database;
using BinaryDiff.Shared.Infrastructure.RelationalDatabase.Repositories;

namespace BinaryDiff.Result.Infrastructure.Repositories
{
    public class InputDifferencesRepository : BaseRepository<InputDifference>, IInputDifferencesRepository
    {
        public InputDifferencesRepository(ResultContext context) : base(context)
        {
        }
    }
}
