namespace IMDbClone.Common.Constants
{
    public static class CacheKeys
    {
        public const string AllMovies = "AllMovies";

        public static string MovieById(int id) => $"Movie_{id}";

        public const string AllRatings = "AllRatings";

        public static string RatingByMovieId(int movieId) => $"Rating_Movie_{movieId}";

        public const string AllReviews = "AllReviews";

        public static string ReviewByMovieId(int movieId) => $"Review_Movie_{movieId}";

        public const string AllWatchlists = "AllWatchlists";

        public static string WatchlistByUserId(string userId) => $"Watchlist_User_{userId}";
    }
}