using System.Linq.Expressions;
using IMDbClone.Core.DTOs.MovieDTOs;
using IMDbClone.Core.Models;
using IMDbClone.Core.Utilities;

namespace IMDbClone.Business.Services.IServices
{
    public interface IMovieService
    {
        Task<PaginatedResult<MovieSummaryDTO>> GetAllMoviesAsync(
            Expression<Func<Movie, bool>>? filter = null,
            Expression<Func<Movie, object>>? orderByExpression = null,
            bool isAscending = true,
            int pageNumber = 1,
            int pageSize = 10);

        Task<MovieDTO> GetMovieByIdAsync(int id);

        Task<MovieDTO> CreateMovieAsync(CreateMovieDTO movieDTO);

        Task<UpdateMovieDTO> UpdateMovieAsync(int id, UpdateMovieDTO movieDTO);

        Task DeleteMovieAsync(int id);

        Task<PaginatedResult<MovieSummaryDTO>> GetTopRatedMoviesAsync(int pageNumber, int pageSize);

        Task<PaginatedResult<MovieSummaryDTO>> GetMostPopularMoviesAsync(int pageNumber, int pageSize);
    }
}