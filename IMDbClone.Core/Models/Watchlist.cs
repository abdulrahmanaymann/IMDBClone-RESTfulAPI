using System.ComponentModel.DataAnnotations.Schema;

namespace IMDbClone.Core.Entities
{
    public class Watchlist
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } = default!;

        public int MovieId { get; set; }

        [ForeignKey("MovieId")]
        public Movie Movie { get; set; } = default!;
    }
}
