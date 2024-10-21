using System.Linq.Expressions;
using AutoMapper;
using IMDbClone.Business.Services.IServices;
using IMDbClone.Common.Constants;
using IMDbClone.Core.DTOs.MovieDTOs;
using IMDbClone.Core.Entities;
using IMDbClone.Core.Exceptions;
using IMDbClone.Core.Utilities;
using IMDbClone.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace IMDbClone.Business.Services
{
    public class MovieService : IMovieService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public MovieService(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<PaginatedResult<MovieDTO>> GetAllMoviesAsync(
            Expression<Func<Movie, bool>>? filter = null,
            Expression<Func<Movie, object>>? orderByExpression = null,
            bool isAscending = true,
            int pageNumber = 1,
            int pageSize = 10)
        {
            if (pageNumber < 1)
            {
                throw new ApiException("Page number must be greater than zero.", HttpStatusCodes.BadRequest);
            }

            if (pageSize < 1)
            {
                throw new ApiException("Page size must be greater than zero.", HttpStatusCodes.BadRequest);
            }

            var cacheKey = CacheKeys.AllMovies;
            _cacheService.Remove(cacheKey);

            return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
            {
                try
                {
                    var movies = await _unitOfWork.Movie.GetAllAsync(
                        filter: filter,
                        includeProperties: "Reviews,Ratings",
                        orderByExpression: orderByExpression ??
                                (isAscending ? m => m.Title : (m => m.Title)),
                        isAscending: isAscending,
                        pageNumber: pageNumber,
                        pageSize: pageSize
                    );

                    movies.Items.AsQueryable().AsSplitQuery();

                    return new PaginatedResult<MovieDTO>
                    {
                        Items = _mapper.Map<IEnumerable<MovieDTO>>(movies.Items),
                        TotalCount = movies.TotalCount,
                        PageNumber = movies.PageNumber,
                        PageSize = movies.PageSize
                    };
                }
                catch (Exception ex)
                {
                    throw new ApiException("An error occurred while retrieving movies.",
                        HttpStatusCodes.InternalServerError, ex);
                }
            });
        }

        public async Task<MovieDTO> GetMovieByIdAsync(int id)
        {
            var cacheKey = CacheKeys.MovieById(id);
            return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
            {
                var movie = await _unitOfWork.Movie.GetAsync(u => u.Id == id, includeProperties: "Reviews,Ratings");
                if (movie == null)
                {
                    throw new ApiException($"Movie with ID {id} not found.", HttpStatusCodes.NotFound);
                }

                return _mapper.Map<MovieDTO>(movie);
            });
        }

        public async Task<MovieDTO> CreateMovieAsync(CreateMovieDTO movieDTO)
        {
            var existingMovie = await _unitOfWork.Movie.GetAsync(u => u.Title == movieDTO.Title);
            if (existingMovie != null)
            {
                throw new ApiException($"Movie with title {movieDTO.Title} already exists.",
                    HttpStatusCodes.Conflict);
            }

            var movie = _mapper.Map<Movie>(movieDTO);
            await _unitOfWork.Movie.AddAsync(movie);

            _cacheService.Remove(CacheKeys.AllMovies);

            return _mapper.Map<MovieDTO>(movie);
        }

        public async Task<UpdateMovieDTO> UpdateMovieAsync(int id, UpdateMovieDTO movieDTO)
        {
            ArgumentNullException.ThrowIfNull(movieDTO);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

            var movie = await _unitOfWork.Movie.GetAsync(u => u.Id == id) ??
                throw new ApiException($"Movie with ID {id} not found.", HttpStatusCodes.NotFound);

            _mapper.Map(movieDTO, movie);
            await _unitOfWork.Movie.UpdateAsync(movie);

            _cacheService.Remove(CacheKeys.MovieById(id));
            _cacheService.Remove(CacheKeys.AllMovies);

            return _mapper.Map<UpdateMovieDTO>(movie);
        }

        public async Task DeleteMovieAsync(int id)
        {
            var movie = await _unitOfWork.Movie.GetAsync(u => u.Id == id) ??
                throw new ApiException($"Movie with ID {id} not found.", HttpStatusCodes.NotFound);

            await _unitOfWork.Movie.RemoveAsync(movie);

            _cacheService.Remove(CacheKeys.MovieById(id));
            _cacheService.Remove(CacheKeys.AllMovies);
        }

        public async Task<IEnumerable<MovieDTO>> GetTopRatedMoviesAsync(int count)
        {
            var cacheKey = CacheKeys.TopRatedMovies(count);

            return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
            {
                var topRatedMovies = await _unitOfWork.Movie.GetTopRatedMoviesAsync(count);
                return _mapper.Map<IEnumerable<MovieDTO>>(topRatedMovies);
            });
        }

        public async Task<IEnumerable<MovieDTO>> GetMostPopularMoviesAsync(int count)
        {
            var cacheKey = CacheKeys.MostPopularMovies(count);

            return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
            {
                var popularMovies = await _unitOfWork.Movie.GetMostPopularMoviesAsync(count);
                return _mapper.Map<IEnumerable<MovieDTO>>(popularMovies);
            });
        }
    }
}