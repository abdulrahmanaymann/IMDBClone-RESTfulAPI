using System.ComponentModel.DataAnnotations;

namespace IMDbClone.Core.DTOs.ReviewDTOs
{
    public class UpdateReviewDTO
    {
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Content must be between 10 and 1000 characters.")]
        public string? Content { get; set; } = string.Empty;
    }
}
