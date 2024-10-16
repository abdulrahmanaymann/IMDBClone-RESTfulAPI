using AutoMapper;
using IMDbClone.Business.Services.IServices;
using IMDbClone.Core.DTOs.MovieDTOs;
using IMDbClone.Core.Entities;
using IMDbClone.DataAccess.Repository.IRepository;

namespace IMDbClone.Business.Services
{
    public class MovieService : IMovieService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MovieService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MovieDTO>> GetAllMoviesAsync()
        {
            try
            {
                var movies = await _unitOfWork.Movie.GetAllAsync();
                return _mapper.Map<IEnumerable<MovieDTO>>(movies);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving movies. Please try again later.", ex);
            }
        }

        public async Task<MovieDTO> GetMovieByIdAsync(int id)
        {
            var movie = await _unitOfWork.Movie.GetAsync(u => u.Id == id);
            if (movie == null)
            {
                throw new KeyNotFoundException($"Movie with ID {id} not found.");
            }

            return _mapper.Map<MovieDTO>(movie);
        }

        public async Task<CreateMovieDTO> CreateMovieAsync(CreateMovieDTO movieDTO)
        {
            var existingMovie = await _unitOfWork.Movie.GetAsync(u => u.Title == movieDTO.Title);
            if (existingMovie != null)
            {
                throw new InvalidOperationException($"Movie with title {movieDTO.Title} already exists.");
            }

            var movie = _mapper.Map<Movie>(movieDTO);
            await _unitOfWork.Movie.AddAsync(movie);

            return _mapper.Map<CreateMovieDTO>(movie);
        }

        public async Task<UpdateMovieDTO> UpdateMovieAsync(int id, UpdateMovieDTO movieDTO)
        {
            if (movieDTO == null)
            {
                throw new ArgumentNullException(nameof(movieDTO));
            }

            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            var movie = await _unitOfWork.Movie.GetAsync(u => u.Id == id);
            if (movie == null)
            {
                throw new KeyNotFoundException($"Movie with ID {id} not found.");
            }

            _mapper.Map(movieDTO, movie);
            await _unitOfWork.Movie.UpdateAsync(movie);

            return _mapper.Map<UpdateMovieDTO>(movie);
        }

        public async Task DeleteMovieAsync(int id)
        {
            var movie = await _unitOfWork.Movie.GetAsync(u => u.Id == id);
            if (movie == null)
            {
                throw new KeyNotFoundException($"Movie with ID {id} not found.");
            }

            await _unitOfWork.Movie.RemoveAsync(movie);
        }
    }
}