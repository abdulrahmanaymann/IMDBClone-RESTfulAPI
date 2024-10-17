using IMDbClone.Core.DTOs.RatingDTOs;

namespace IMDbClone.Business.Services.IServices
{
    public interface IRatingService
    {
        Task<IEnumerable<RatingDTO>> GetAllRatingsAsync();

        Task<RatingDTO> GetRatingByIdAsync(int id);

        Task<RatingDTO> CreateRatingAsync(CreateRatingDTO ratingDTO);

        Task<UpdateRatingDTO> UpdateRatingAsync(int id, UpdateRatingDTO ratingDTO);

        Task DeleteRatingAsync(int id);
    }
}
