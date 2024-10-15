namespace IMDbClone.Core.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public List<Rating> Ratings { get; set; } = new List<Rating>();

        public List<Review> Reviews { get; set; } = new List<Review>();
    }
}
