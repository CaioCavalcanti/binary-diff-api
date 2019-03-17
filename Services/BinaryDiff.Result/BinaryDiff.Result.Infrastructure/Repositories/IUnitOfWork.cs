using BinaryDiff.Result.Infrastructure.Database;
using System.Threading.Tasks;

namespace BinaryDiff.Result.Infrastructure.Repositories
{
    public interface IUnitOfWork
    {
        ResultContext Context { get; }

        Task SaveChangesAsync();
    }
}
