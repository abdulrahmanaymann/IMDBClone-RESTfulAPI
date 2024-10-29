using IMDbClone.Core.Models;
using IMDbClone.DataAccess.Data;
using IMDbClone.DataAccess.Repository.IRepository;

namespace IMDbClone.DataAccess.Repository
{
    public class WatchlistRepository : Repository<Watchlist>, IWatchlistRepository
    {
        private readonly ApplicationDbContext _context;

        public WatchlistRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
