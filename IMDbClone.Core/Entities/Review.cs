using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMDbClone.Core.Entities
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public int MovieId { get; set; }

        [ForeignKey("MovieId")]
        public Movie Movie { get; set; }
    }
}
