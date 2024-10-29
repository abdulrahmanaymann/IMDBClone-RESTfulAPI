using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMDbClone.Core.Models
{
    public class Rating
    {
        public int Id { get; set; }

        [Column(TypeName = "decimal(3, 1)")]
        [Range(1, 10, ErrorMessage = "Rating must be between 1 and 10.")]
        public decimal Score { get; set; }

        public int MovieId { get; set; }

        [ForeignKey(nameof(MovieId))]
        public Movie Movie { get; set; } = default!;

        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
