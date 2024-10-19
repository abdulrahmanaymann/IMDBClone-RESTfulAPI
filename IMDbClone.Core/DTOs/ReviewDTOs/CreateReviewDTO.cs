using System.ComponentModel.DataAnnotations;

namespace IMDbClone.Core.DTOs.ReviewDTOs
{
    public class CreateReviewDTO
    {
        [Required]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Content must be between 10 and 1000 characters.")]
        public string Content { get; set; } = string.Empty;

        [Required]
        public int MovieId { get; set; }
    }
}
