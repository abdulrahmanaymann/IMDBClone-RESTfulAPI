using AutoMapper;
using IMDbClone.Business.Services.IServices;
using IMDbClone.Common.Constants;
using IMDbClone.Core.DTOs.ReviewDTOs;
using IMDbClone.Core.Entities;
using IMDbClone.Core.Exceptions;
using IMDbClone.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;

namespace IMDbClone.Business.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<ReviewDTO>> GetAllReviewsAsync()
        {
            var cacheKey = CacheKeys.AllReviews;
            try
            {
                var cachedReviews = await _cacheService.GetOrCreateAsync(cacheKey, async () =>
                {
                    var paginatedResult = await _unitOfWork.Review.GetAllAsync(includeProperties: "User,Movie");
                    return _mapper.Map<IEnumerable<ReviewDTO>>(paginatedResult.Items);
                });

                return cachedReviews;
            }
            catch (Exception ex)
            {
                throw new ApiException("An error occurred while retrieving reviews.",
                    HttpStatusCodes.InternalServerError, ex);
            }
        }

        public async Task<ReviewDTO> GetReviewByIdAsync(int id)
        {
            var cacheKey = CacheKeys.ReviewByMovieId(id);
            try
            {
                var cachedReview = await _cacheService.GetOrCreateAsync(cacheKey, async () =>
                {
                    var review = await _unitOfWork.Review.GetAsync(u => u.Id == id, includeProperties: "User,Movie");
                    if (review == null)
                    {
                        throw new KeyNotFoundException($"Review with ID {id} not found.");
                    }
                    return _mapper.Map<ReviewDTO>(review);
                });

                return cachedReview;
            }
            catch (Exception ex)
            {
                throw new ApiException("An error occurred while retrieving the review.",
                        HttpStatusCodes.InternalServerError, ex);
            }
        }

        public async Task<ReviewDTO> CreateReviewAsync(CreateReviewDTO reviewDTO, string userId)
        {
            if (reviewDTO == null)
            {
                throw new ApiException("Review data cannot be null.", StatusCodes.Status400BadRequest);
            }

            var existingReview = await _unitOfWork.Review.GetAsync(r => r.UserId == userId &&
                                                                    r.MovieId == reviewDTO.MovieId);
            if (existingReview != null)
            {
                throw new ApiException($"Review for the movie with ID {reviewDTO.MovieId} " +
                    $"by user with ID {userId} already exists.", StatusCodes.Status409Conflict);
            }

            var review = _mapper.Map<Review>(reviewDTO);
            review.UserId = userId;

            await _unitOfWork.Review.AddAsync(review);
            _cacheService.Remove(CacheKeys.AllReviews);

            return _mapper.Map<ReviewDTO>(review);
        }

        public async Task<UpdateReviewDTO> UpdateReviewAsync(int id, UpdateReviewDTO reviewDTO, string userId)
        {
            if (reviewDTO == null)
            {
                throw new ApiException("Review data is missing.", StatusCodes.Status400BadRequest);
            }

            if (id <= 0)
            {
                throw new ApiException("Review ID is invalid.", StatusCodes.Status400BadRequest);
            }

            var review = await _unitOfWork.Review.GetAsync(u => u.Id == id && u.UserId == userId);
            if (review == null)
            {
                throw new ApiException($"Review with ID {id} not found.", StatusCodes.Status404NotFound);
            }

            _mapper.Map(reviewDTO, review);
            await _unitOfWork.Review.UpdateAsync(review);

            _cacheService.Remove(CacheKeys.ReviewByMovieId(id));
            _cacheService.Remove(CacheKeys.AllReviews);

            return _mapper.Map<UpdateReviewDTO>(review);
        }

        public async Task DeleteReviewAsync(int id, string userId)
        {
            var review = await _unitOfWork.Review.GetAsync(u => u.Id == id && u.UserId == userId);
            if (review == null)
            {
                throw new ApiException($"Review with ID {id} not found.", StatusCodes.Status404NotFound);
            }

            await _unitOfWork.Review.RemoveAsync(review);
            _cacheService.Remove(CacheKeys.ReviewByMovieId(id));
            _cacheService.Remove(CacheKeys.AllReviews);
        }

    }
}