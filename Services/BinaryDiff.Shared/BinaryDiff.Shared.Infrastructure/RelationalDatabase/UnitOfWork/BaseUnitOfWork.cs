using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BinaryDiff.Shared.Infrastructure.RelationalDatabase
{
    public class BaseUnitOfWork<TDbContext> : IBaseUnitOfWork
        where TDbContext : DbContext
    {
        protected readonly TDbContext context;

        public BaseUnitOfWork(TDbContext context)
        {
            this.context = context;
        }

        public void Dispose()
        {
            context?.Dispose();
        }

        public Task SaveChangesAsync()
        {
            return context.SaveChangesAsync();
        }
    }
}
