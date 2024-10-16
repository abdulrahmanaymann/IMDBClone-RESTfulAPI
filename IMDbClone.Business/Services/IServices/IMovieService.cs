using IMDbClone.Core.DTOs.MovieDTOs;

namespace IMDbClone.Business.Services.IServices
{
    public interface IMovieService
    {
        Task<IEnumerable<MovieDTO>> GetAllMoviesAsync();

        Task<MovieDTO> GetMovieByIdAsync(int id);

        Task<CreateMovieDTO> CreateMovieAsync(CreateMovieDTO movieDTO);

        Task<UpdateMovieDTO> UpdateMovieAsync(int id, UpdateMovieDTO movieDTO);

        Task DeleteMovieAsync(int id);
    }
}
