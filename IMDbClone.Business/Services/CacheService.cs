using IMDbClone.Business.Services.IServices;
using IMDbClone.Common.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace IMDbClone.Business.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly CacheSettings _cacheSettings;

        public CacheService(IMemoryCache cache, IOptions<CacheSettings> cacheSettings)
        {
            _cache = cache;
            _cacheSettings = cacheSettings.Value;
        }

        public async Task<T> GetOrCreateAsync<T>(
                string cacheKey,
                Func<Task<T>> createItemAsync,
                TimeSpan? absoluteExpiration = null,
                TimeSpan? slidingExpiration = null)
        {
            ArgumentNullException.ThrowIfNull(createItemAsync);

            if (!_cache.TryGetValue(cacheKey, out T cachedItem))
            {
                cachedItem = await createItemAsync();

                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = absoluteExpiration ?? _cacheSettings.DefaultAbsoluteExpiration,
                    SlidingExpiration = slidingExpiration ?? _cacheSettings.DefaultSlidingExpiration
                };

                _cache.Set(cacheKey, cachedItem, cacheOptions);
            }
            return cachedItem;
        }


        public void Remove(string cacheKey)
        {
            _cache.Remove(cacheKey);
        }
    }
}
