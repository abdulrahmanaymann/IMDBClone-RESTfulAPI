using System.ComponentModel.DataAnnotations;

namespace IMDbClone.Core.DTOs.WatchlistDTOs
{
    public class WatchlistDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int MovieId { get; set; }

        [Required]
        public string MovieTitle { get; set; } = string.Empty;
    }
}
