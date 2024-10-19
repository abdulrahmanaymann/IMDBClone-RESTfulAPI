using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMDbClone.Core.DTOs.RatingDTOs
{
    public class CreateRatingDTO
    {
        [Required]
        [Column(TypeName = "decimal(3, 1)")]
        [Range(1, 10, ErrorMessage = "Rating must be between 1 and 10.")]
        public decimal Score { get; set; }

        [Required]
        public int MovieId { get; set; }
    }
}
