using IMDbClone.Core.DTOs.RatingDTOs;

namespace IMDbClone.Business.Services.IServices
{
    public interface IRatingService
    {
        Task<IEnumerable<RatingDTO>> GetAllRatingsAsync();

        Task<RatingDTO> GetRatingByIdAsync(int id);

        Task<RatingDTO> CreateRatingAsync(CreateRatingDTO ratingDTO, string userId);

        Task<UpdateRatingDTO> UpdateRatingAsync(int id, UpdateRatingDTO ratingDTO, string userId);

        Task DeleteRatingAsync(int id, string userId);
    }
}
