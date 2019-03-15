namespace BinaryDiff.Infrastructure.Repositories
{
    public interface IMemoryRepository<TKey, TValue>
    {
        void Save(TKey key, TValue value);

        TValue Find(TKey key);
    }
}
