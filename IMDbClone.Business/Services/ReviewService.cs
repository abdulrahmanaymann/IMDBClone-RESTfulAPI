using AutoMapper;
using IMDbClone.Business.Services.IServices;
using IMDbClone.Core.DTOs.ReviewDTOs;
using IMDbClone.Core.Entities;
using IMDbClone.DataAccess.Repository.IRepository;

namespace IMDbClone.Business.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReviewDTO>> GetAllReviewsAsync()
        {
            try
            {
                var reviews = await _unitOfWork.Review.GetAllAsync(includeProperties: "User,Movie");
                return _mapper.Map<IEnumerable<ReviewDTO>>(reviews);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving reviews. Please try again later.", ex);
            }
        }

        public async Task<ReviewDTO> GetReviewByIdAsync(int id)
        {
            var review = await _unitOfWork.Review.GetAsync(u => u.Id == id, includeProperties: "User,Movie");
            if (review == null)
            {
                throw new KeyNotFoundException($"Review with ID {id} not found.");
            }

            return _mapper.Map<ReviewDTO>(review);
        }

        public async Task<ReviewDTO> CreateReviewAsync(CreateReviewDTO reviewDTO)
        {
            var existingReview = await _unitOfWork.Review.GetAsync(u => u.UserId == reviewDTO.UserId && u.MovieId == reviewDTO.MovieId);
            if (existingReview != null)
            {
                throw new InvalidOperationException($"Review for the movie with ID {reviewDTO.MovieId} by user with ID {reviewDTO.UserId} already exists.");
            }

            var review = _mapper.Map<Review>(reviewDTO);
            await _unitOfWork.Review.AddAsync(review);

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

            return _mapper.Map<UpdateReviewDTO>(review);
        }

        public async Task DeleteReviewAsync(int id)
        {
            var review = await _unitOfWork.Review.GetAsync(u => u.Id == id);
            if (review == null)
            {
                throw new KeyNotFoundException($"Review with ID {id} not found.");
            }

            await _unitOfWork.Review.RemoveAsync(review);
        }
    }
}