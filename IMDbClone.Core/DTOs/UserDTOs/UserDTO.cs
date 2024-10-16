using System.ComponentModel.DataAnnotations;
using IMDbClone.Core.DTOs.RatingDTOs;
using IMDbClone.Core.DTOs.ReviewDTOs;

namespace IMDbClone.Core.DTOs.UserDTOs
{
    public class UserDTO
    {
        public int Id { get; set; }

        [Required]

        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public List<RatingDTO> Ratings { get; set; } = new List<RatingDTO>();

        public List<ReviewDTO> Reviews { get; set; } = new List<ReviewDTO>();
    }
}
