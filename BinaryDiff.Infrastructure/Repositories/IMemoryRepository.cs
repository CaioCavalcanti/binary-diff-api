namespace BinaryDiff.Infrastructure.Repositories
{
    /// <summary>
    /// Simple in memory repository based on key/value pairs
    /// </summary>
    /// <typeparam name="TKey">Entity key for a given type TKey</typeparam>
    /// <typeparam name="TValue">Entity value for a given type TValue</typeparam>
    public interface IMemoryRepository<TKey, TValue>
    {
        /// <summary>
        /// Saves an object in a repository
        /// </summary>
        /// <param name="key">Entity key</param>
        /// <param name="value">Entity of type TValue</param>
        /// <exception cref="System.InvalidOperationException">Throws exception for null key or value</exception>
        void Save(TKey key, TValue value);

        /// <summary>
        /// Retrieves an object on repository dictionary in a given key
        /// </summary>
        /// <param name="key">Entity of type TValue or its default value</param>
        /// <returns>Entity of type TValue</returns>
        /// <exception cref="System.InvalidOperationException">Throws exception for null key</exception>
        TValue Find(TKey key);

        /// <summary>
        /// Removes key from repository
        /// </summary>
        /// <param name="key">Entity key of type TKey</param>
        void Remove(TKey key);
    }
}
