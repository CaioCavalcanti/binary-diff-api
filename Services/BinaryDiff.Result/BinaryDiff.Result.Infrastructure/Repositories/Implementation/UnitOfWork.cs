using BinaryDiff.Result.Infrastructure.Database;
using System.Threading.Tasks;

namespace BinaryDiff.Result.Infrastructure.Repositories.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(ResultContext context)
        {
            Context = context;
        }

        public ResultContext Context { get; }

        public async Task SaveChangesAsync()
        {
            await Context.SaveChangesAsync();
        }
    }
}
