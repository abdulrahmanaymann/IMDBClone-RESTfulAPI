using IMDbClone.Core.DTOs.ReviewDTOs;

namespace IMDbClone.Business.Services.IServices
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDTO>> GetAllReviewsAsync();

        Task<ReviewDTO> GetReviewByIdAsync(int id);

        Task<ReviewDTO> CreateReviewAsync(CreateReviewDTO reviewDTO);

        Task<UpdateReviewDTO> UpdateReviewAsync(int id, UpdateReviewDTO reviewDTO);

        Task DeleteReviewAsync(int id);
    }
}
