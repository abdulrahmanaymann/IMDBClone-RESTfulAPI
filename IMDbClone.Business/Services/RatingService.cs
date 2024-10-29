using AutoMapper;
using IMDbClone.Business.Services.IServices;
using IMDbClone.Common.Constants;
using IMDbClone.Core.DTOs.RatingDTOs;
using IMDbClone.Core.Exceptions;
using IMDbClone.Core.Models;
using IMDbClone.Core.Utilities;
using IMDbClone.DataAccess.Repository.IRepository;

namespace IMDbClone.Business.Services
{
    public class RatingService : IRatingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public RatingService(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<RatingDTO>> GetAllRatingsAsync()
        {
            var cacheKey = CacheKeys.AllRatings;

            try
            {
                var cachedRatings = await _cacheService.GetOrCreateAsync(cacheKey, async () =>
                {
                    var paginatedResult = await _unitOfWork.Rating.GetAllAsync(includeProperties: "User,Movie");
                    var paginatedRatingsDTO = _mapper.Map<PaginatedResult<RatingDTO>>(paginatedResult);
                    return paginatedRatingsDTO.Items;
                });

                return cachedRatings;
            }
            catch (Exception ex)
            {
                throw new ApiException("An error occurred while retrieving ratings.",
                    HttpStatusCodes.InternalServerError, ex);
            }
        }

        public async Task<RatingDTO> GetRatingByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ApiException("Rating ID must be greater than zero.", HttpStatusCodes.BadRequest);
            }

            var cacheKey = CacheKeys.RatingByMovieId(id);
            var cachedRating = await _cacheService.GetOrCreateAsync(cacheKey, async () =>
            {
                var rating = await _unitOfWork.Rating.GetAsync(u => u.Id == id, includeProperties: "User,Movie");
                if (rating == null)
                {
                    throw new ApiException($"Rating with ID {id} not found.", HttpStatusCodes.NotFound);
                }
                return _mapper.Map<RatingDTO>(rating);
            });

            return cachedRating;
        }

        public async Task<RatingDTO> CreateRatingAsync(CreateRatingDTO ratingDTO, string userId)
        {
            if (ratingDTO == null)
            {
                throw new ApiException("Rating data cannot be null.", HttpStatusCodes.BadRequest);
            }

            var userExists = await _unitOfWork.User.GetAsync(u => u.Id == userId);
            if (userExists == null)
            {
                throw new ApiException($"User with ID {userId} not found.", HttpStatusCodes.NotFound);
            }

            var existingRating = await _unitOfWork.Rating.GetAsync(u => u.UserId == userId &&
                                                    u.MovieId == ratingDTO.MovieId);
            if (existingRating != null)
            {
                throw new ApiException($"Rating for the movie with ID {ratingDTO.MovieId} already exists.",
                        HttpStatusCodes.Conflict);
            }

            var rating = _mapper.Map<Rating>(ratingDTO);
            rating.UserId = userId;

            await _unitOfWork.Rating.AddAsync(rating);
            _cacheService.Remove(CacheKeys.AllRatings);

            return _mapper.Map<RatingDTO>(rating);
        }

        public async Task<UpdateRatingDTO> UpdateRatingAsync(int id, UpdateRatingDTO ratingDTO, string userId)
        {
            if (ratingDTO == null)
            {
                throw new ApiException("Rating data cannot be null.", HttpStatusCodes.BadRequest);
            }

            if (id <= 0)
            {
                throw new ApiException("Rating ID must be greater than zero.", HttpStatusCodes.BadRequest);
            }

            var rating = await _unitOfWork.Rating.GetAsync(u => u.Id == id && u.UserId == userId);
            if (rating == null)
            {
                throw new ApiException($"Rating with ID {id} not found.", HttpStatusCodes.NotFound);
            }

            _mapper.Map(ratingDTO, rating);
            await _unitOfWork.Rating.UpdateAsync(rating);

            _cacheService.Remove(CacheKeys.RatingByMovieId(id));
            _cacheService.Remove(CacheKeys.AllRatings);

            return _mapper.Map<UpdateRatingDTO>(rating);
        }

        public async Task DeleteRatingAsync(int id, string userId)
        {
            if (id <= 0)
            {
                throw new ApiException("Rating ID must be greater than zero.", HttpStatusCodes.BadRequest);
            }

            var rating = await _unitOfWork.Rating.GetAsync(u => u.Id == id && u.UserId == userId);
            if (rating == null)
            {
                throw new ApiException($"Rating with ID {id} not found.", HttpStatusCodes.NotFound);
            }

            await _unitOfWork.Rating.RemoveAsync(rating);

            _cacheService.Remove(CacheKeys.RatingByMovieId(id));
            _cacheService.Remove(CacheKeys.AllRatings);
        }
    }
}