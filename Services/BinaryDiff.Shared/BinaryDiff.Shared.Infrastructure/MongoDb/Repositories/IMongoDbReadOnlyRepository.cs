using BinaryDiff.Shared.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BinaryDiff.Shared.Infrastructure.MongoDb.Repositories
{
    public interface IMongoDbReadOnlyRepository<TDocument>
        where TDocument : BaseDocument
    {
        Task<IList<TDocument>> FindAsync(Expression<Func<TDocument, bool>> predicate);
    }
}
