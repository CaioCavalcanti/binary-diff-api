using BinaryDiff.Shared.Domain.Models;
using BinaryDiff.Shared.Infrastructure.MongoDb.Context;
using MongoDB.Driver;
using System;

namespace BinaryDiff.Shared.Infrastructure.MongoDb.Repositories
{
    public class MongoDbBaseRepository<TDocument>
        where TDocument : BaseDocument
    {
        protected readonly IMongoCollection<TDocument> collection;

        public MongoDbBaseRepository(IMongoDbContext mongoDbContext, string collectionName)
        {
            if (string.IsNullOrEmpty(collectionName))
            {
                throw new InvalidOperationException($"Collection name was not informed for {typeof(TDocument).Name} repository");
            }

            collection = mongoDbContext.Database.GetCollection<TDocument>(collectionName);
        }
    }
}
