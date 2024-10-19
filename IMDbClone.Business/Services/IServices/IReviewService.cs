using IMDbClone.Core.DTOs.ReviewDTOs;

namespace IMDbClone.Business.Services.IServices
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDTO>> GetAllReviewsAsync();

        Task<ReviewDTO> GetReviewByIdAsync(int id);

        Task<ReviewDTO> CreateReviewAsync(CreateReviewDTO reviewDTO, string userId);

        Task<UpdateReviewDTO> UpdateReviewAsync(int id, UpdateReviewDTO reviewDTO, string userId);

        Task DeleteReviewAsync(int id, string userId);
    }
}
