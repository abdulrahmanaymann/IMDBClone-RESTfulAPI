using IMDbClone.DataAccess.Data;
using IMDbClone.DataAccess.Repository.IRepository;

namespace IMDbClone.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IMovieRepository Movie { get; private set; }
        public IRatingRepository Rating { get; private set; }
        public IReviewRepository Review { get; private set; }

        public IUserRepository User { get; private set; }

        public IWatchlistRepository Watchlist { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Movie = new MovieRepository(_context);
            Rating = new RatingRepository(_context);
            Review = new ReviewRepository(_context);
            User = new UserRepository(_context);
            Watchlist = new WatchlistRepository(_context);
        }
    }
}