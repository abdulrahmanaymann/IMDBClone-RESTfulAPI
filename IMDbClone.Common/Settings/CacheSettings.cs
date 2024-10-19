namespace IMDbClone.Common.Settings
{
    public class CacheSettings
    {
        public TimeSpan DefaultAbsoluteExpiration { get; set; }
        public TimeSpan DefaultSlidingExpiration { get; set; }
    }
}