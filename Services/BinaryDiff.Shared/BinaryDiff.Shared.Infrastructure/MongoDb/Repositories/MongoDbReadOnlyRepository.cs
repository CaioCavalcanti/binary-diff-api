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

        public async Task<IList<TDocument>> FindAsync(Expression<Func<TDocument, bool>> predicate)
        {
            var filter = new FilterDefinitionBuilder<TDocument>().Where(predicate);

            var result = await collection.FindAsync(filter);

            return result.ToList();
        }
    }
}
