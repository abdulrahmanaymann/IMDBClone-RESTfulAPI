using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using IMDbClone.Core.Enums;
using Newtonsoft.Json.Converters;

namespace IMDbClone.Core.DTOs.MovieDTOs
{
    public class MovieSummaryDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public GenreEnum Genre { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        [Required]
        [StringLength(50000, MinimumLength = 10, ErrorMessage = "Synopsis must be between 10 and 500 characters.")]
        public string Synopsis { get; set; } = string.Empty;

        [Url]
        public string? PosterUrl { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Director must be between 3 and 100 characters.")]
        public string Director { get; set; } = string.Empty;

        public int ReviewCount { get; set; } = 0;

        public decimal AverageRating { get; set; } = 0;
    }
}