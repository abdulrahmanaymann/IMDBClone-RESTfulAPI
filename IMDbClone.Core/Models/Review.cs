using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMDbClone.Core.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } = default!;

        public int MovieId { get; set; }

        [ForeignKey("MovieId")]
        public Movie Movie { get; set; } = default!;
    }
}
