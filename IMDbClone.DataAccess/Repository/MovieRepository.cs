using IMDbClone.Core.Entities;
using IMDbClone.DataAccess.Data;
using IMDbClone.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace IMDbClone.DataAccess.Repository
{
    internal class MovieRepository : Repository<Movie>, IMovieRepository
    {
        private readonly ApplicationDbContext _context;

        public MovieRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Movie>> GetMostPopularMoviesAsync(int count)
        {
            return await _context.Movies
                .OrderByDescending(m => m.Ratings.Count())
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetTopRatedMoviesAsync(int count)
        {
            return await _context.Movies
                .OrderByDescending(m => m.Ratings.Average(r => r.Score))
                .Take(count)
                .ToListAsync();
        }

        public async Task UpdateAsync(Movie movie)
        {
            _context.Movies.Update(movie);
            await _context.SaveChangesAsync();
        }
    }
}
