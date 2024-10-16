using System.ComponentModel.DataAnnotations;
using IMDbClone.Core.Enums;
using IMDbClone.Core.Validation;

namespace IMDbClone.Core.DTOs.MovieDTOs
{
    public class UpdateMovieDTO
    {
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters.")]
        public string? Title { get; set; }

        public Genre? Genre { get; set; }

        public DateTime? ReleaseDate { get; set; }

        [StringLength(500, MinimumLength = 10, ErrorMessage = "Synopsis must be between 10 and 500 characters.")]
        public string? Synopsis { get; set; }

        [Url]
        public string? PosterUrl { get; set; }

        [StringLength(100, MinimumLength = 3, ErrorMessage = "Director must be between 3 and 100 characters.")]
        public string? Director { get; set; }

        [StringLength(500, MinimumLength = 10, ErrorMessage = "Cast must be between 10 and 500 characters.")]
        [ValidCast]
        public List<string>? Cast { get; set; } = new List<string>();

        [StringLength(50, MinimumLength = 3, ErrorMessage = "Language must be between 3 and 50 characters.")]
        public string? Language { get; set; }

        [Range(1, 300, ErrorMessage = "Duration must be between 1 and 300 minutes.")]
        public int? Duration { get; set; }

        [Url]
        public string? TrailerUrl { get; set; }
    }
}
