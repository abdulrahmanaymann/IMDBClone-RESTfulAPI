using IMDbClone.Core.Entities;

namespace IMDbClone.DataAccess.Repository.IRepository
{
    public interface IMovieRepository : IRepository<Movie>
    {
        Task UpdateAsync(Movie movie);
    }
}
