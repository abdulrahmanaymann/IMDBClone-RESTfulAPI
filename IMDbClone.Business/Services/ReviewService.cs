using AutoMapper;
using IMDbClone.Business.Services.IServices;
using IMDbClone.Core.DTOs.ReviewDTOs;
using IMDbClone.Core.Entities;
using IMDbClone.DataAccess.Repository.IRepository;
using Microsoft.Extensions.Caching.Memory;

namespace IMDbClone.Business.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<IEnumerable<ReviewDTO>> GetAllReviewsAsync()
        {
            var cacheKey = "allReviews";
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<ReviewDTO> cachedReviews))
            {
                try
                {
                    var reviews = await _unitOfWork.Review.GetAllAsync(includeProperties: "User,Movie");
                    cachedReviews = _mapper.Map<IEnumerable<ReviewDTO>>(reviews);

                    // Set cache options
                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(30))
                        .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                    _cache.Set(cacheKey, cachedReviews, cacheOptions);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("An error occurred while retrieving reviews. " +
                        "Please try again later.", ex);
                }
            }

            return cachedReviews;

        }

        public async Task<ReviewDTO> GetReviewByIdAsync(int id)
        {
            var cacheKey = $"review_{id}";
            if (!_cache.TryGetValue(cacheKey, out ReviewDTO cachedReview))
            {
                var review = await _unitOfWork.Review.GetAsync(u => u.Id == id, includeProperties: "User,Movie");
                if (review == null)
                {
                    throw new KeyNotFoundException($"Review with ID {id} not found.");
                }

                cachedReview = _mapper.Map<ReviewDTO>(review);

                // Set cache options
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(30))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _cache.Set(cacheKey, cachedReview, cacheOptions);
            }

            return cachedReview;
        }

        public async Task<ReviewDTO> CreateReviewAsync(CreateReviewDTO reviewDTO)
        {
            var existingReview = await _unitOfWork.Review.GetAsync(u => u.UserId == reviewDTO.UserId && u.MovieId == reviewDTO.MovieId);
            if (existingReview != null)
            {
                throw new InvalidOperationException($"Review for the movie with ID {reviewDTO.MovieId} " +
                    $"by user with ID {reviewDTO.UserId} already exists.");
            }

            var review = _mapper.Map<Review>(reviewDTO);
            await _unitOfWork.Review.AddAsync(review);

            _cache.Remove("allReviews");

            return _mapper.Map<ReviewDTO>(review);
        }

        public async Task<UpdateReviewDTO> UpdateReviewAsync(int id, UpdateReviewDTO reviewDTO)
        {
            if (reviewDTO == null)
            {
                throw new ArgumentNullException(nameof(reviewDTO), "Review data is missing.");
            }

            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Review ID is invalid.");
            }

            var review = await _unitOfWork.Review.GetAsync(u => u.Id == id);
            if (review == null)
            {
                throw new KeyNotFoundException($"Review with ID {id} not found.");
            }

            _mapper.Map(reviewDTO, review);
            await _unitOfWork.Review.UpdateAsync(review);

            _cache.Remove($"review_{id}");
            _cache.Remove("allReviews");

            return _mapper.Map<UpdateReviewDTO>(review);
        }

        public async Task DeleteReviewAsync(int id)
        {
            var review = await _unitOfWork.Review.GetAsync(u => u.Id == id);
            if (review == null)
            {
                throw new KeyNotFoundException($"Review with ID {id} not found.");
            }

            _cache.Remove($"review_{id}");
            _cache.Remove("allReviews");

            await _unitOfWork.Review.RemoveAsync(review);
        }
    }
}