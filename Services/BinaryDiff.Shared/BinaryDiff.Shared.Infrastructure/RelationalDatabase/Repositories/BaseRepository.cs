using BinaryDiff.Shared.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BinaryDiff.Shared.Infrastructure.RelationalDatabase.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T>
        where T : BaseEntity
    {
        protected readonly DbSet<T> dbSet;

        public BaseRepository(DbContext context)
        {
            dbSet = context.Set<T>();
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public async Task<T> FindAsync(Guid id)
        {
            return await dbSet.FindAsync(id);
        }

        public IQueryable<T> Get(Expression<Func<T, bool>> predicate = null)
        {
            return dbSet.Where(predicate).AsQueryable();
        }
    }
}
