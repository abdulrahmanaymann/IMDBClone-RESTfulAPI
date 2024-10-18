using AutoMapper;
using IMDbClone.Business.Services.IServices;
using IMDbClone.Core.DTOs.RatingDTOs;
using IMDbClone.Core.Entities;
using IMDbClone.DataAccess.Repository.IRepository;
using Microsoft.Extensions.Caching.Memory;

namespace IMDbClone.Business.Services
{
    public class RatingService : IRatingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public RatingService(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<IEnumerable<RatingDTO>> GetAllRatingsAsync()
        {
            var cacheKey = "AllRatings";
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<RatingDTO> cachedRatings))
            {
                try
                {
                    var ratings = await _unitOfWork.Rating.GetAllAsync(includeProperties: "User,Movie");
                    cachedRatings = _mapper.Map<IEnumerable<RatingDTO>>(ratings);

                    // Set cache options
                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(30))
                        .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                    _cache.Set(cacheKey, cachedRatings, cacheOptions);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("An error occurred while retrieving ratings. " +
                        "Please try again later.", ex);
                }
            }

            return cachedRatings;
        }

        public async Task<RatingDTO> GetRatingByIdAsync(int id)
        {
            var cacheKey = $"rating_{id}";
            if (!_cache.TryGetValue(cacheKey, out RatingDTO cachedRating))
            {
                var rating = await _unitOfWork.Rating.GetAsync(u => u.Id == id, includeProperties: "User,Movie");
                if (rating == null)
                {
                    throw new KeyNotFoundException($"Rating with ID {id} not found.");
                }

                cachedRating = _mapper.Map<RatingDTO>(rating);

                // Set cache options
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(30))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _cache.Set(cacheKey, cachedRating, cacheOptions);
            }

            return cachedRating;
        }

        public async Task<RatingDTO> CreateRatingAsync(CreateRatingDTO ratingDTO)
        {
            if (ratingDTO == null)
            {
                throw new ArgumentNullException(nameof(ratingDTO), "Rating data cannot be null.");
            }

            var existingRating = await _unitOfWork.Rating.GetAsync(u => u.UserId == ratingDTO.UserId && u.MovieId == ratingDTO.MovieId);
            if (existingRating != null)
            {
                throw new InvalidOperationException($"Rating for movie ID {ratingDTO.MovieId} " +
                    $"by user ID {ratingDTO.UserId} already exists.");
            }

            var rating = _mapper.Map<Rating>(ratingDTO);
            await _unitOfWork.Rating.AddAsync(rating);

            _cache.Remove("allRatings");

            return _mapper.Map<RatingDTO>(rating);
        }

        public async Task<UpdateRatingDTO> UpdateRatingAsync(int id, UpdateRatingDTO ratingDTO)
        {
            if (ratingDTO == null)
            {
                throw new ArgumentNullException(nameof(ratingDTO), "Rating data cannot be null.");
            }

            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Rating ID must be greater than zero.");
            }

            var rating = await _unitOfWork.Rating.GetAsync(u => u.Id == id);
            if (rating == null)
            {
                throw new KeyNotFoundException($"Rating with ID {id} not found.");
            }

            _mapper.Map(ratingDTO, rating);
            await _unitOfWork.Rating.UpdateAsync(rating);

            _cache.Remove($"rating_{id}");
            _cache.Remove("allRatings");

            return _mapper.Map<UpdateRatingDTO>(rating);
        }

        public async Task DeleteRatingAsync(int id)
        {
            var rating = await _unitOfWork.Rating.GetAsync(u => u.Id == id);
            if (rating == null)
            {
                throw new KeyNotFoundException($"Rating with ID {id} not found.");
            }

            await _unitOfWork.Rating.RemoveAsync(rating);

            _cache.Remove($"rating_{id}");
            _cache.Remove("allRatings");
        }
    }
}