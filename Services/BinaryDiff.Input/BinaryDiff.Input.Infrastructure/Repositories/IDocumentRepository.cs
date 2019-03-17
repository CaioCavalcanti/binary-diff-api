using BinaryDiff.Input.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BinaryDiff.Input.Infrastructure.Repositories
{
    public interface IDocumentRepository<TDocument>
        where TDocument : BaseDocument
    {
        Task<IList<TDocument>> FindAsync(Expression<Func<TDocument, bool>> predicate);

        Task AddOneAsync(TDocument document);
    }
}
