using IMDbClone.Core.Entities;
using IMDbClone.DataAccess.Data;
using IMDbClone.DataAccess.Repository.IRepository;

namespace IMDbClone.DataAccess.Repository
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task UpdateAsync(Review review)
        {
            _context.Reviews.Update(review);
            await SaveChangesAsync();
        }
    }
}
