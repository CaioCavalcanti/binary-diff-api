using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace BinaryDiff.API.Helpers
{
    public static class CacheExtensions
    {
        public static async Task CacheAsync(this IDistributedCache cache, string key, object content)
        {
            var serialized = JsonConvert.SerializeObject(content);

            await cache.SetAsync(key, Encoding.UTF8.GetBytes(serialized), _cacheEntryOptions);
        }

        public static async Task<T> GetAsync<T>(this IDistributedCache cache, string key)
        {
            var instanceOnCache = await cache.GetAsync(key);

            if (instanceOnCache == null)
            {
                return default(T);
            }

            var strInstance = Encoding.UTF8.GetString(instanceOnCache);

            return JsonConvert.DeserializeObject<T>(strInstance);
        }

        private static DistributedCacheEntryOptions _cacheEntryOptions
            => new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(20));
    }
}
