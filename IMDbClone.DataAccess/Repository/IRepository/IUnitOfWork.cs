namespace IMDbClone.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IMovieRepository Movie { get; }

        IRatingRepository Rating { get; }

        IReviewRepository Review { get; }

        IUserRepository User { get; }

        IWatchlistRepository Watchlist { get; }
    }
}
