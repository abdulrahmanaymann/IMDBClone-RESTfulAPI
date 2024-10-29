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

        public static string TopRatedMovies(int pageNumber, int pageSize)
                    => $"top_rated_movies_{pageNumber}_{pageSize}";

        public static string MostPopularMovies(int pageNumber, int pageSize)
                    => $"most_popular_movies_{pageNumber}_{pageSize}";

        public static string UserById(string userId) => $"User_{userId}";
        public static string AllUsers(int pageNumber, int pageSize) => $"AllUsers_{pageNumber}_{pageSize}";
    }
}