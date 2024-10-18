using System.Linq.Expressions;
using AutoMapper;
using IMDbClone.Business.Services.IServices;
using IMDbClone.Core.DTOs.MovieDTOs;
using IMDbClone.Core.Entities;
using IMDbClone.Core.Utilities;
using IMDbClone.DataAccess.Repository.IRepository;
using Microsoft.Extensions.Caching.Memory;

namespace IMDbClone.Business.Services
{
    public class MovieService : IMovieService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public MovieService(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<PaginatedResult<MovieDTO>> GetAllMoviesAsync(
                Expression<Func<Movie, bool>>? filter = null,
                Expression<Func<Movie, object>>? orderByExpression = null,
                bool isAscending = true,
                int pageNumber = 1,
                int pageSize = 10)
        {
            var cacheKey = "AllMovies";
            if (!_cache.TryGetValue(cacheKey, out PaginatedResult<MovieDTO> cachedResult))
            {
                try
                {
                    var movies = await _unitOfWork.Movie.GetAllAsync(
                        filter: filter,
                        includeProperties: "Reviews,Ratings",
                        orderByExpression: orderByExpression,
                        isAscending: isAscending,
                        pageNumber: pageNumber,
                        pageSize: pageSize
                    );

                    cachedResult = new PaginatedResult<MovieDTO>
                    {
                        Items = _mapper.Map<IEnumerable<MovieDTO>>(movies.Items),
                        TotalCount = movies.TotalCount,
                        PageNumber = movies.PageNumber,
                        PageSize = movies.PageSize
                    };

                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10)) // Refresh if inactive for 10 minutes
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1)); // Refresh every hour

                    _cache.Set(cacheKey, cachedResult, cacheEntryOptions); // Cache the result
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("An error occurred while retrieving movies. " +
                        "Please try again later.", ex);
                }
            }

            return cachedResult;
        }

        public async Task<MovieDTO> GetMovieByIdAsync(int id)
        {
            var cacheKey = $"Movie_{id}";
            if (!_cache.TryGetValue(cacheKey, out MovieDTO cachedMovie))
            {
                var movie = await _unitOfWork.Movie.GetAsync(u => u.Id == id, includeProperties: "Reviews,Ratings");
                if (movie == null)
                {
                    throw new KeyNotFoundException($"Movie with ID {id} not found.");
                }

                cachedMovie = _mapper.Map<MovieDTO>(movie);
                // Store the movie in cache
                _cache.Set(cacheKey, cachedMovie);
            }

            return cachedMovie;
        }

        public async Task<MovieDTO> CreateMovieAsync(CreateMovieDTO movieDTO)
        {
            var existingMovie = await _unitOfWork.Movie.GetAsync(u => u.Title == movieDTO.Title);
            if (existingMovie != null)
            {
                throw new InvalidOperationException($"Movie with title {movieDTO.Title} already exists.");
            }

            var movie = _mapper.Map<Movie>(movieDTO);
            await _unitOfWork.Movie.AddAsync(movie);

            // Clear cache after creating a new movie
            _cache.Remove("AllMovies"); // Clear AllMovies cache to ensure the new movie is included

            return _mapper.Map<MovieDTO>(movie);
        }

        public async Task<UpdateMovieDTO> UpdateMovieAsync(int id, UpdateMovieDTO movieDTO)
        {
            ArgumentNullException.ThrowIfNull(movieDTO);

            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

            var movie = await _unitOfWork.Movie.GetAsync(u => u.Id == id) ??
                throw new KeyNotFoundException($"Movie with ID {id} not found.");
            _mapper.Map(movieDTO, movie);
            await _unitOfWork.Movie.UpdateAsync(movie);

            // Clear the specific movie cache
            _cache.Remove($"Movie_{id}"); // Remove cache for the updated movie

            // Also clear the AllMovies cache to ensure consistency
            _cache.Remove("AllMovies");

            return _mapper.Map<UpdateMovieDTO>(movie);
        }

        public async Task DeleteMovieAsync(int id)
        {
            var movie = await _unitOfWork.Movie.GetAsync(u => u.Id == id) ??
                throw new KeyNotFoundException($"Movie with ID {id} not found.");
            await _unitOfWork.Movie.RemoveAsync(movie);

            // Clear cache for the deleted movie
            _cache.Remove($"Movie_{id}"); // Remove cache for the deleted movie

            // Also clear the AllMovies cache to ensure consistency
            _cache.Remove("AllMovies");
        }
    }
}