using BinaryDiff.Input.Domain.Models;
using BinaryDiff.Input.Infrastructure.Helpers;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace BinaryDiff.Input.Infrastructure.Repositories.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IMongoCollection<Diff> _diffCollection;
        private readonly IMongoCollection<InputData> _inputCollection;

        public UnitOfWork(IConfiguration configuration)
        {
            var database = configuration.GetMongoDatabase();

            _diffCollection = database.GetCollection<Diff>("diff");
            _inputCollection = database.GetCollection<InputData>("inputData");

            DiffRepository = new Repository<Diff>(_diffCollection);
            InputRepository = new Repository<InputData>(_inputCollection);
        }

        public IRepository<InputData> InputRepository { get; }

        public IRepository<Diff> DiffRepository { get; }
    }
}
