using IMDbClone.Core.Entities;
using IMDbClone.Core.Utilities;
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

        public async Task<PaginatedResult<Movie>> GetTopRatedMoviesAsync(
            int pageNumber,
            int pageSize,
            bool trackChanges = true,
            string? includeProperties = null)
        {
            var query = trackChanges
                ? _context.Movies.AsQueryable()
                : _context.Movies.AsNoTracking();

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(m => m.Ratings.Average(r => r.Score))
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Movie>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = items
            };
        }

        public async Task<PaginatedResult<Movie>> GetMostPopularMoviesAsync(
            int pageNumber,
            int pageSize,
            bool trackChanges = true,
            string? includeProperties = null)
        {
            var query = trackChanges
                ? _context.Movies.AsQueryable()
                : _context.Movies.AsNoTracking();

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(m => m.Ratings.Count())
                .AsSplitQuery()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Movie>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = items
            };
        }

        public async Task UpdateAsync(Movie movie)
        {
            _context.Movies.Update(movie);
            await _context.SaveChangesAsync();
        }
    }
}
