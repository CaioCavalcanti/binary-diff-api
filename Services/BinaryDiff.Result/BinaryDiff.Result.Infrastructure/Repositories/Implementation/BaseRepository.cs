using BinaryDiff.Result.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BinaryDiff.Result.Infrastructure.Repositories.Implementation
{
    public class BaseRepository<T> : IBaseRepository<T>
        where T : BaseModel
    {
        private readonly IUnitOfWork _uow;
        protected readonly DbSet<T> dbSet;

        public BaseRepository(IUnitOfWork uow)
        {
            _uow = uow;
            dbSet = _uow.Context.Set<T>();
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

        public async Task SaveChangesAsync()
        {
            await _uow.SaveChangesAsync();
        }
    }
}
