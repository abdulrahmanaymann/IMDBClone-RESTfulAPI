using IMDbClone.Core.Entities;
using IMDbClone.DataAccess.Data;
using IMDbClone.DataAccess.Repository.IRepository;

namespace IMDbClone.DataAccess.Repository
{
    public class RatingRepository : Repository<Rating>, IRatingRepository
    {
        private readonly ApplicationDbContext _context;

        public RatingRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task UpdateAsync(Rating rating)
        {
            _context.Ratings.Update(rating);
            await _context.SaveChangesAsync();
        }
    }
}
