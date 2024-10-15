using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMDbClone.Core.Entities
{
    public class Rating
    {
        public int Id { get; set; }

        [Column(TypeName = "decimal(3, 1)")]
        [Range(1, 10, ErrorMessage = "Rating must be between 1 and 10.")]
        public decimal Score { get; set; }

        public int MovieId { get; set; }

        [ForeignKey(nameof(MovieId))]
        public Movie Movie { get; set; }

        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
