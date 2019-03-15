using System;
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
            if (key == null)
            {
                throw new InvalidOperationException();
            }

            if (_repository.TryGetValue(key, out var value))
            {
                return value;
            }

            return default(TValue);
        }

        public void Save(TKey key, TValue value)
        {
            if (key == null)
            {
                throw new InvalidOperationException();
            }

            _repository[key] = value;
        }
    }
}
