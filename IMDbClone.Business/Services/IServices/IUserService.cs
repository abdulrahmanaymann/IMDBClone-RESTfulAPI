using System.Linq.Expressions;
using IMDbClone.Core.DTOs.UserDTOs;
using IMDbClone.Core.Models;
using IMDbClone.Core.Utilities;

namespace IMDbClone.Business.Services.IServices
{
    public interface IUserService
    {
        Task<bool> IsUniqueUser(string username);

        Task<List<string>> GetAllRolesAsync();

        Task<List<string>> GetUserRolesAsync(UserDTO user);

        Task AddUserToRoleAsync(UserDTO user, string roleName);

        Task UpdateRolesAsync(UserDTO user, IEnumerable<string> roles);

        Task RemoveUserFromRoleAsync(UserDTO user, string roleName);

        Task<PaginatedResult<UserDTO>> GetAllUsersAsync(
             Expression<Func<ApplicationUser, bool>>? filter = null,
             string? includeProperties = null,
             bool isAscending = true,
             int pageNumber = 1,
             int pageSize = 10);

        Task<UserDTO> GetUserByIdAsync(string userId);

        Task RemoveUserAsync(UserDTO user);
    }
}