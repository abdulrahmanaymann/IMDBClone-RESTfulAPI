using IMDbClone.Common.Constants;
using IMDbClone.Core.Exceptions;
using IMDbClone.Core.Models;
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
            if (pageNumber < 1)
            {
                throw new ApiException("Page number must be greater than zero.", HttpStatusCodes.BadRequest);
            }

            if (pageSize < 1)
            {
                throw new ApiException("Page size must be greater than zero.", HttpStatusCodes.BadRequest);
            }

            var query = trackChanges
                ? _context.Movies.AsQueryable()
                : _context.Movies.AsNoTracking();

            query = ApplyIncludes(includeProperties, query);

            var totalCount = await query.CountAsync();

            var items = await query
                .Select(m => new
                {
                    Movie = m,
                    AverageRating = m.Ratings.Any() ? m.Ratings.Average(r => r.Score) : 0
                })
                .OrderByDescending(m => m.AverageRating)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Movie>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = items.Select(x => x.Movie).ToList()
            };
        }

        public async Task<PaginatedResult<Movie>> GetMostPopularMoviesAsync(
            int pageNumber,
            int pageSize,
            bool trackChanges = true,
            string? includeProperties = null)
        {
            if (pageNumber < 1)
            {
                throw new ApiException("Page number must be greater than zero.", HttpStatusCodes.BadRequest);
            }

            if (pageSize < 1)
            {
                throw new ApiException("Page size must be greater than zero.", HttpStatusCodes.BadRequest);
            }

            var query = trackChanges
                ? _context.Movies.AsQueryable()
                : _context.Movies.AsNoTracking();

            query = ApplyIncludes(includeProperties, query);

            var totalCount = await query.CountAsync();

            var items = await query
                .Select(m => new
                {
                    Movie = m,
                    ReviewCount = m.Reviews.Count()
                })
                .OrderByDescending(m => m.ReviewCount)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Movie>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = items.Select(x => x.Movie).ToList()
            };
        }

        public async Task UpdateAsync(Movie movie)
        {
            _context.Movies.Update(movie);
            await _context.SaveChangesAsync();
        }
    }
}