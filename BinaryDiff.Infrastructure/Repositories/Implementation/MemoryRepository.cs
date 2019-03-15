using System.Collections.Generic;

namespace BinaryDiff.Infrastructure.Repositories.Implementation
{
    public class MemoryRepository<TKey, TValue> : IMemoryRepository<TKey, TValue>
    {
        private IDictionary<TKey, TValue> _repository;

        public MemoryRepository()
        {
            _repository = new Dictionary<TKey, TValue>();
        }

        public TValue Find(TKey key)
        {
            return _repository[key];
        }

        public void Save(TKey key, TValue value)
        {
            _repository[key] = value;
        }
    }
}
