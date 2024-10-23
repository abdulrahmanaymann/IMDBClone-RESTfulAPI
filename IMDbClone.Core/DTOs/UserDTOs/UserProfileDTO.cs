using System.ComponentModel.DataAnnotations;
using IMDbClone.Core.Validation;

namespace IMDbClone.Core.DTOs.UserDTOs
{
    public class UserProfileDTO
    {
        public string UserName { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [FullName]
        public string Name { get; set; } = string.Empty;

        //public List<RatingDTO> Ratings { get; set; } = new List<RatingDTO>();

        //public List<ReviewDTO> Reviews { get; set; } = new List<ReviewDTO>();

        //public List<MovieDTO> FavoriteMovies { get; set; } = new List<MovieDTO>();
    }
}
