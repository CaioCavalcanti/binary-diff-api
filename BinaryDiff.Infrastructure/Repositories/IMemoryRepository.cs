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
        /// Saves an object in a dictionary in memory
        /// </summary>
        /// <param name="key">Entity key</param>
        /// <param name="value">Entity of type TValue</param>
        /// <exception cref="System.InvalidOperationException">Throws exception for null key or value</exception>
        void Save(TKey key, TValue value);

        /// <summary>
        /// Retrieves a value from in memory dictionary for a given key
        /// </summary>
        /// <param name="key">Entity of type TValue or its default value</param>
        /// <returns>Entity of type TValue</returns>
        /// <exception cref="System.InvalidOperationException">Throws exception for null key</exception>
        TValue Find(TKey key);
    }
}
