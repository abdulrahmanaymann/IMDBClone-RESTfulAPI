namespace IMDbClone.Common.Settings
{

    public class CacheSettings
    {
        public TimeSpan DefaultAbsoluteExpiration { get; set; } = TimeSpan.FromMinutes(60);
        public TimeSpan DefaultSlidingExpiration { get; set; } = TimeSpan.FromMinutes(30);
    }
}