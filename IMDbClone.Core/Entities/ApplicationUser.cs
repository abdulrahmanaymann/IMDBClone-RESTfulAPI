using Microsoft.AspNetCore.Identity;

namespace IMDbClone.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public List<Rating> Ratings { get; set; } = new List<Rating>();

        public List<Review> Reviews { get; set; } = new List<Review>();

        public List<Watchlist> Watchlists { get; set; } = new List<Watchlist>();
    }
}