using System.ComponentModel.DataAnnotations;

namespace IMDbClone.Core.DTOs.WatchlistDTOs
{
    public class CreateWatchlistDTO
    {
        [Required]
        public int MovieId { get; set; }
    }
}
