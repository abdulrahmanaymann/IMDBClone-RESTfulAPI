using IMDbClone.Core.Enums;
using IMDbClone.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IMDbClone.DataAccess.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : IdentityDbContext<ApplicationUser>(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Movie>()
            .Property(m => m.Genre)
            .HasConversion(
            v => v.ToString(),
            v => (GenreEnum)Enum.Parse(typeof(GenreEnum), v));

            modelBuilder.Entity<Rating>()
                .HasIndex(r => r.MovieId)
                .HasDatabaseName("IX_Ratings_MovieId");

            modelBuilder.Entity<Rating>()
                .HasIndex(r => r.UserId)
                .HasDatabaseName("IX_Ratings_UserId");

            modelBuilder.Entity<Review>()
                .HasIndex(r => r.MovieId)
                .HasDatabaseName("IX_Reviews_MovieId");

            modelBuilder.Entity<Review>()
                .HasIndex(r => r.UserId)
                .HasDatabaseName("IX_Reviews_UserId");

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<Movie> Movies { get; set; }

        public DbSet<Rating> Ratings { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<Watchlist> Watchlists { get; set; }
    }
}