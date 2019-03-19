using BinaryDiff.Shared.Domain.Models;
using BinaryDiff.Shared.Infrastructure.MongoDb.Context;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BinaryDiff.Shared.Infrastructure.MongoDb.Repositories
{
    public class MongoDbReadOnlyRepository<TDocument> : MongoDbBaseRepository<TDocument>, IMongoDbReadOnlyRepository<TDocument>
        where TDocument : BaseDocument
    {
        public MongoDbReadOnlyRepository(IMongoDbContext mongoDbContext, string collectionName)
            : base(mongoDbContext, collectionName)
        {
        }

        public Task<List<TDocument>> FindAsync(Expression<Func<TDocument, bool>> predicate)
        {
            return collection
                .Find(predicate)
                .ToListAsync();
        }

        public Task<TDocument> FindAsync(string id)
        {
            return collection
                .Find(document => document.Id == id)
                .FirstOrDefaultAsync();
        }
    }
}
