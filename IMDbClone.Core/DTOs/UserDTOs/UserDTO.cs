using IMDbClone.Core.DTOs.MovieDTOs;
using IMDbClone.Core.DTOs.RatingDTOs;
using IMDbClone.Core.DTOs.ReviewDTOs;

namespace IMDbClone.Core.DTOs.UserDTOs
{
    public class UserDTO
    {
        public string Id { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public List<RatingDTO> Ratings { get; set; } = new List<RatingDTO>();

        public List<ReviewDTO> Reviews { get; set; } = new List<ReviewDTO>();

        public List<MovieDTO> FavoriteMovies { get; set; } = new List<MovieDTO>();
    }
}