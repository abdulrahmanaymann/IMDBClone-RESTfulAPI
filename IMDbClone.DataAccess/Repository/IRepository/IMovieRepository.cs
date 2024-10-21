using IMDbClone.Core.Entities;

namespace IMDbClone.DataAccess.Repository.IRepository
{
    public interface IMovieRepository : IRepository<Movie>
    {
        Task UpdateAsync(Movie movie);

        Task<IEnumerable<Movie>> GetTopRatedMoviesAsync(int count);

        Task<IEnumerable<Movie>> GetMostPopularMoviesAsync(int count);
    }
}