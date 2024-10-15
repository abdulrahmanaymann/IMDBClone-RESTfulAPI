﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IMDbClone.Core.Enums;

namespace IMDbClone.Core.Entities
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public Genre Genre { get; set; }  // Enum for genre like Action, Drama, etc.
        public DateTime ReleaseDate { get; set; } = DateTime.UtcNow;
        public string Synopsis { get; set; } = string.Empty; // Short description of the movie
        [Url]
        public string? PosterUrl { get; set; }
        public string Director { get; set; } = string.Empty;
        public string Cast { get; set; } = string.Empty;  // Comma-separated list of actors or a related entity
        public string Language { get; set; } = string.Empty;
        public int Duration { get; set; }  // Duration in minutes
        [Url]
        public string? TrailerUrl { get; set; } // URL for the movie trailer
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<Rating> Ratings { get; set; } = new List<Rating>();
        public List<Review> Reviews { get; set; } = new List<Review>();

        // Dynamic property to calculate the average rating at runtime (not stored in the database)
        [NotMapped]
        public decimal AverageRating => Ratings.Count > 0 ? Ratings.Average(r => r.Score) : 0;
    }
}
