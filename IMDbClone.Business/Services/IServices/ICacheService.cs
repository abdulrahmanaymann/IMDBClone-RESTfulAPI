namespace IMDbClone.Business.Services.IServices
{
    public interface ICacheService
    {
        public Task<T> GetOrCreateAsync<T>(string cacheKey, Func<Task<T>> createItemAsync,
            TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null);

        void Remove(string cacheKey);
    }
}