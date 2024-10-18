using IMDbClone.Core.Entities;
using IMDbClone.DataAccess.Data;
using IMDbClone.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
namespace IMDbClone.DataAccess.Repository
{
    public class UserRepository : Repository<ApplicationUser>, IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> IsUniqueUser(string username)
        {
            return await _context.Users.AnyAsync(u => u.UserName == username) == false;
        }

    }
}