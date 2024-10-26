using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using IMDbClone.Core.DTOs.RatingDTOs;
using IMDbClone.Core.DTOs.ReviewDTOs;
using IMDbClone.Core.Enums;
using Newtonsoft.Json.Converters;

namespace IMDbClone.Core.DTOs.MovieDTOs
{
    public class MovieDTO
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

        [Required]
        //[ValidCast]
        public List<string> Cast { get; set; } = new List<string>();

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Language must be between 3 and 50 characters.")]
        public string Language { get; set; } = string.Empty;

        [Required]
        [Range(1, 300, ErrorMessage = "Duration must be between 1 and 300 minutes.")]
        public int Duration { get; set; }

        [Url]
        public string? TrailerUrl { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public decimal? AverageRating { get; set; }

        public List<ReviewDTO> Reviews { get; set; } = new List<ReviewDTO>();

        public List<RatingDTO> Ratings { get; set; } = new List<RatingDTO>();

        public int ReviewCount => Reviews?.Count ?? 0;
    }
}