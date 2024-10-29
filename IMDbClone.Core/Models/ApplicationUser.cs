using Microsoft.AspNetCore.Identity;

namespace IMDbClone.Core.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; } = string.Empty;

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        public List<Rating> Ratings { get; set; } = new List<Rating>();

        public List<Review> Reviews { get; set; } = new List<Review>();

        public List<Watchlist> Watchlists { get; set; } = new List<Watchlist>();
    }
}