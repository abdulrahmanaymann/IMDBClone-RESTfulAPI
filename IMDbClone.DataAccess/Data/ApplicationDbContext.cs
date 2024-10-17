using IMDbClone.Core.Entities;
using IMDbClone.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace IMDbClone.DataAccess.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Users
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, UserName = "user1", Password = "password1" },  // Add appropriate default passwords
                new User { Id = 2, UserName = "user2", Password = "password2" }
            );

            // Seed Movies
            modelBuilder.Entity<Movie>().HasData(
                new Movie
                {
                    Id = 1,
                    Title = "Inception",
                    Genre = Genre.Action, // Set the genre appropriately
                    ReleaseDate = new DateTime(2010, 7, 16),
                    Synopsis = "A thief who steals corporate secrets through the use of dream-sharing technology is given the inverse task of planting an idea into the mind of a CEO.",
                    PosterUrl = "http://example.com/inception.jpg",
                    Director = "Christopher Nolan",
                    Cast = "Leonardo DiCaprio, Joseph Gordon-Levitt, Ellen Page",
                    Language = "English",
                    Duration = 148,
                    TrailerUrl = "http://example.com/inception_trailer.mp4"
                },
                new Movie
                {
                    Id = 2,
                    Title = "The Matrix",
                    Genre = Genre.SciFi, // Set the genre appropriately
                    ReleaseDate = new DateTime(1999, 3, 31),
                    Synopsis = "A computer hacker learns from mysterious rebels about the true nature of his reality and his role in the war against its controllers.",
                    PosterUrl = "http://example.com/the_matrix.jpg",
                    Director = "The Wachowskis",
                    Cast = "Keanu Reeves, Laurence Fishburne, Carrie-Anne Moss",
                    Language = "English",
                    Duration = 136,
                    TrailerUrl = "http://example.com/the_matrix_trailer.mp4"
                }
            );

            // Seed Ratings
            modelBuilder.Entity<Rating>().HasData(
                new Rating { Id = 1, MovieId = 1, UserId = 1, Score = 9 },
                new Rating { Id = 2, MovieId = 1, UserId = 2, Score = 8 },
                new Rating { Id = 3, MovieId = 2, UserId = 1, Score = 10 }
            );

            // Seed Reviews
            modelBuilder.Entity<Review>().HasData(
                new Review { Id = 1, MovieId = 1, UserId = 1, Content = "Amazing movie!" },
                new Review { Id = 2, MovieId = 1, UserId = 2, Content = "Really good!" },
                new Review { Id = 3, MovieId = 2, UserId = 1, Content = "A groundbreaking film." }
            );
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Review> Reviews { get; set; }
    }
}
