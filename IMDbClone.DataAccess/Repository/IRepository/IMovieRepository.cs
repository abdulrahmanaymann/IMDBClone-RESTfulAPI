using IMDbClone.Core.Models;
using IMDbClone.Core.Utilities;

namespace IMDbClone.DataAccess.Repository.IRepository
{
    public interface IMovieRepository : IRepository<Movie>
    {
        Task UpdateAsync(Movie movie);

        Task<PaginatedResult<Movie>> GetTopRatedMoviesAsync(
            int pageNumber,
            int pageSize,
            bool trackChanges = true,
            string? includeProperties = null);

        Task<PaginatedResult<Movie>> GetMostPopularMoviesAsync(
            int pageNumber,
            int pageSize,
            bool trackChanges = true,
            string? includeProperties = null);
    }
}