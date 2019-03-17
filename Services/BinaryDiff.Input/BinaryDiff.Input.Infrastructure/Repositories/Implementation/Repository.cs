using BinaryDiff.Input.Domain.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BinaryDiff.Input.Infrastructure.Repositories.Implementation
{
    public class Repository<TDocument> : IRepository<TDocument>
        where TDocument : BaseDocument
    {
        private readonly IMongoCollection<TDocument> _collection;

        public Repository(IMongoCollection<TDocument> collection)
        {
            _collection = collection;
        }

        public async Task AddOneAsync(TDocument document)
        {
            await _collection.InsertOneAsync(document);
        }

        public async Task<IList<TDocument>> FindAsync(Expression<Func<TDocument, bool>> predicate)
        {
            var filter = new FilterDefinitionBuilder<TDocument>().Where(predicate);

            var result = await _collection.FindAsync(filter);

            return result.ToList();
        }

        public async Task<bool> UpdateOneAsync(string documentId, TDocument document)
        {
            var result = await _collection.ReplaceOneAsync(_ => _._id == documentId, document);

            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}
