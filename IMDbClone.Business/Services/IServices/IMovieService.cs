using System.Linq.Expressions;
using IMDbClone.Core.DTOs.MovieDTOs;
using IMDbClone.Core.Entities;
using IMDbClone.Core.Utilities;

namespace IMDbClone.Business.Services.IServices
{
    public interface IMovieService
    {
        Task<PaginatedResult<MovieDTO>> GetAllMoviesAsync(
            Expression<Func<Movie, bool>>? filter = null,
            Expression<Func<Movie, object>>? orderByExpression = null,
            bool isAscending = true,
            int pageNumber = 1,
            int pageSize = 10
            );

        Task<MovieDTO> GetMovieByIdAsync(int id);

        Task<MovieDTO> CreateMovieAsync(CreateMovieDTO movieDTO);

        Task<UpdateMovieDTO> UpdateMovieAsync(int id, UpdateMovieDTO movieDTO);

        Task DeleteMovieAsync(int id);

        Task<IEnumerable<MovieDTO>> GetTopRatedMoviesAsync(int count);

        Task<IEnumerable<MovieDTO>> GetMostPopularMoviesAsync(int count);
    }
}
