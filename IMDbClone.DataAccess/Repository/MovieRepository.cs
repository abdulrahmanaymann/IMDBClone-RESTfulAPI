using IMDbClone.Core.Entities;
using IMDbClone.DataAccess.Data;
using IMDbClone.DataAccess.Repository.IRepository;

namespace IMDbClone.DataAccess.Repository
{
    internal class MovieRepository : Repository<Movie>, IMovieRepository
    {
        private readonly ApplicationDbContext _context;

        public MovieRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task UpdateAsync(Movie movie)
        {
            _context.Movies.Update(movie);
            await _context.SaveChangesAsync();
        }
    }
}
