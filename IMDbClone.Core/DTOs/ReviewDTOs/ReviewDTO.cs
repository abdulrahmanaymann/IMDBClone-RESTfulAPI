using System.ComponentModel.DataAnnotations;

namespace IMDbClone.Core.DTOs.ReviewDTOs
{
    public class ReviewDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Content must be between 10 and 1000 characters.")]
        public string Content { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public string UserId { get; set; }

        [Required]
        public int MovieId { get; set; }
    }
}
