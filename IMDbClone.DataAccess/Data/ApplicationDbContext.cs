using IMDbClone.Core.Entities;
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
                .HasIndex(m => m.Title)
                .HasDatabaseName("IX_Movies_Title");

            modelBuilder.Entity<Movie>()
                .HasIndex(m => m.Synopsis)
                .HasDatabaseName("IX_Movies_Synopsis");

            modelBuilder.Entity<Movie>()
                .HasIndex(m => m.Genre)
                .HasDatabaseName("IX_Movies_Genre");

            modelBuilder.Entity<Movie>()
                .HasIndex(m => m.Director)
                .HasDatabaseName("IX_Movies_Director");

            modelBuilder.Entity<Movie>()
                .HasIndex(m => m.Language)
                .HasDatabaseName("IX_Movies_Language");

            modelBuilder.Entity<Movie>()
                .HasIndex(m => m.ReleaseDate)
                .HasDatabaseName("IX_Movies_ReleaseDate");

            modelBuilder.Entity<Movie>()
                .HasIndex(m => new { m.Genre, m.ReleaseDate })
                .HasDatabaseName("IX_Movies_Genre_ReleaseDate");

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<Movie> Movies { get; set; }

        public DbSet<Rating> Ratings { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<Watchlist> Watchlists { get; set; }
    }
}