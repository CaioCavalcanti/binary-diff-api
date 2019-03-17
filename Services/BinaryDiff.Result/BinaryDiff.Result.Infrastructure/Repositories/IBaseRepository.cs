using BinaryDiff.Result.Domain.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BinaryDiff.Result.Infrastructure.Repositories
{
    public interface IBaseRepository<T>
        where T : BaseModel
    {
        void Add(T entity);

        Task<T> FindAsync(Guid id);

        IQueryable<T> Get(Expression<Func<T, bool>> predicate = null);

        Task SaveChangesAsync();
    }
}
