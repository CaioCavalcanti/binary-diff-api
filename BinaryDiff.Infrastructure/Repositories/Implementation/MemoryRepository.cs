using System;
using System.Collections.Generic;

namespace BinaryDiff.Infrastructure.Repositories.Implementation
{
    /// <summary>
    /// Simple in memory repository based on key/value pairs
    /// </summary>
    /// <typeparam name="TKey">Entity key for a given type TKey</typeparam>
    /// <typeparam name="TValue">Entity value for a given type TValue</typeparam>
    public class MemoryRepository<TKey, TValue> : IMemoryRepository<TKey, TValue>
    {
        private IDictionary<TKey, TValue> _items;

        /// <summary>
        /// Initialize the dictionary of TKey and TValue provided
        /// </summary>
        public MemoryRepository()
        {
            _items = new Dictionary<TKey, TValue>();
        }

        /// <summary>
        /// Initialize the dictionary of TKey and TValue with a given seed data
        /// </summary>
        public MemoryRepository(IDictionary<TKey, TValue> seedData)
        {
            _items = seedData;
        }

        /// <summary>
        /// Retrieves a value from in memory dictionary for a given key
        /// </summary>
        /// <param name="key">Entity key of type TKey</param>
        /// <returns>Entity of type TValue or its default value</returns>
        /// <exception cref="System.InvalidOperationException">Throws exception for null key</exception>
        public TValue Find(TKey key)
        {
            if (key == null)
            {
                throw new InvalidOperationException();
            }

            if (_items.TryGetValue(key, out var value))
            {
                return value;
            }

            return default(TValue);
        }

        /// <summary>
        /// Saves an object in a dictionary in memory
        /// </summary>
        /// <param name="key">Entity key</param>
        /// <param name="value">Entity of type TValue</param>
        /// <exception cref="System.InvalidOperationException">Throws exception for null key or value</exception>
        public void Save(TKey key, TValue value)
        {
            if (key == null || value == null)
            {
                throw new InvalidOperationException();
            }

            _items[key] = value;
        }

        /// <summary>
        /// Removes key from repository
        /// </summary>
        /// <param name="key">Entity key of type TKey</param>
        public void Remove(TKey key)
        {
            if (key == null)
            {
                throw new InvalidOperationException();
            }

            _items.Remove(key);
        }
    }
}
