using IMDbClone.Core.Models;
using IMDbClone.DataAccess.Data;
using IMDbClone.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
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

        public async Task<List<string>> GetAllRolesAsync()
        {
            return await _context.Roles
                                 .AsNoTracking()
                                 .Select(r => r.Name!)
                                 .ToListAsync();
        }

        public async Task<List<string>> GetUserRolesAsync(ApplicationUser user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var userRoles = await _context.UserRoles
                                          .Where(u => u.UserId == user.Id)
                                          .Join(_context.Roles,
                                                ur => ur.RoleId,
                                                r => r.Id,
                                                (ur, r) => r.Name)
                                          .ToListAsync();
            return userRoles!;
        }

        public async Task AddUserToRoleAsync(ApplicationUser user, string roleName)
        {
            ArgumentNullException.ThrowIfNull(user);

            var role = await _context.Roles.SingleOrDefaultAsync(r => r.Name == roleName) ??
                    throw new ArgumentException($"Role '{roleName}' does not exist.", nameof(roleName));
            var userRole = new IdentityUserRole<string>
            {
                UserId = user.Id,
                RoleId = role.Id
            };

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRolesAsync(ApplicationUser user, IEnumerable<string> roles)
        {
            ArgumentNullException.ThrowIfNull(user);

            var currentRoles = await _context.UserRoles.Where(ur => ur.UserId == user.Id).ToListAsync();
            _context.UserRoles.RemoveRange(currentRoles);

            foreach (var roleName in roles)
            {
                var role = await _context.Roles.SingleOrDefaultAsync(r => r.Name == roleName);
                if (role != null)
                {
                    var userRole = new IdentityUserRole<string>
                    {
                        UserId = user.Id,
                        RoleId = role.Id
                    };
                    _context.UserRoles.Add(userRole);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveUserFromRoleAsync(ApplicationUser user, string roleName)
        {
            ArgumentNullException.ThrowIfNull(user);

            var role = await _context.Roles.SingleOrDefaultAsync(r => r.Name == roleName);
            if (role == null)
            {
                throw new ArgumentException($"Role '{roleName}' does not exist.", nameof(roleName));
            }

            var userRole = await _context.UserRoles.SingleOrDefaultAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id);
            if (userRole != null)
            {
                _context.UserRoles.Remove(userRole);
                await _context.SaveChangesAsync();
            }
        }
    }
}