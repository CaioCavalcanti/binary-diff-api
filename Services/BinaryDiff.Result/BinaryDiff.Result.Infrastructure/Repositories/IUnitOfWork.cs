using BinaryDiff.Shared.Infrastructure.RelationalDatabase;

namespace BinaryDiff.Result.Infrastructure.Repositories
{
    public interface IUnitOfWork : IBaseUnitOfWork
    {
        IDiffResultsRepository DiffResultsRepository { get; }

        IInputDifferencesRepository InputDifferencesRepository { get; }
    }
}
