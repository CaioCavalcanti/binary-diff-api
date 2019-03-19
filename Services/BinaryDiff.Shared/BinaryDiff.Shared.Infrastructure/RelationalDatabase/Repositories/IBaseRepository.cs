using BinaryDiff.Shared.Domain.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BinaryDiff.Shared.Infrastructure.RelationalDatabase.Repositories
{
    public interface IBaseRepository<T>
        where T : BaseEntity
    {
        void Add(T entity);

        Task<T> FindAsync(Guid id);

        IQueryable<T> Get(Expression<Func<T, bool>> predicate = null);
    }
}
