using IMDbClone.Core.Models;

namespace IMDbClone.DataAccess.Repository.IRepository
{
    public interface IUserRepository : IRepository<ApplicationUser>
    {
        Task<bool> IsUniqueUser(string username);

        Task<List<string>> GetUserRolesAsync(ApplicationUser user);

        Task AddUserToRoleAsync(ApplicationUser user, string roleName);

        Task RemoveUserFromRoleAsync(ApplicationUser user, string roleName);

        Task UpdateRolesAsync(ApplicationUser user, IEnumerable<string> roles);

        Task<List<string>> GetAllRolesAsync();
    }
}