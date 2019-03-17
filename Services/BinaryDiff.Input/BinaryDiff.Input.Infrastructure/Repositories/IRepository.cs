using BinaryDiff.Input.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BinaryDiff.Input.Infrastructure.Repositories
{
    public interface IRepository<TDocument>
        where TDocument : BaseDocument
    {
        Task<IList<TDocument>> FindAsync(Expression<Func<TDocument, bool>> predicate);

        Task AddOneAsync(TDocument document);

        Task<bool> UpdateOneAsync(string documentId, TDocument document);
    }
}
