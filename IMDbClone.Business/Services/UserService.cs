using System.Linq.Expressions;
using AutoMapper;
using IMDbClone.Business.Services.IServices;
using IMDbClone.Common.Constants;
using IMDbClone.Core.DTOs.UserDTOs;
using IMDbClone.Core.Models;
using IMDbClone.Core.Utilities;
using IMDbClone.DataAccess.Repository.IRepository;

namespace IMDbClone.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<bool> IsUniqueUser(string username)
        {
            return await _unitOfWork.User.IsUniqueUser(username);
        }

        public async Task<List<string>> GetAllRolesAsync()
        {
            return await _unitOfWork.User.GetAllRolesAsync();
        }

        public async Task<List<string>> GetUserRolesAsync(UserDTO user)
        {
            var appUser = _mapper.Map<ApplicationUser>(user);
            return await _unitOfWork.User.GetUserRolesAsync(appUser);
        }

        public async Task AddUserToRoleAsync(UserDTO user, string roleName)
        {
            ArgumentNullException.ThrowIfNull(user);
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException("Role name cannot be null or empty.", nameof(roleName));

            var appUser = _mapper.Map<ApplicationUser>(user);
            await _unitOfWork.User.AddUserToRoleAsync(appUser, roleName);
        }

        public async Task UpdateRolesAsync(UserDTO user, IEnumerable<string> roles)
        {
            ArgumentNullException.ThrowIfNull(user);
            ArgumentNullException.ThrowIfNull(roles);

            var appUser = _mapper.Map<ApplicationUser>(user);
            await _unitOfWork.User.UpdateRolesAsync(appUser, roles);
        }

        public async Task RemoveUserFromRoleAsync(UserDTO user, string roleName)
        {
            ArgumentNullException.ThrowIfNull(user);
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException("Role name cannot be null or empty.", nameof(roleName));

            var appUser = _mapper.Map<ApplicationUser>(user);
            await _unitOfWork.User.RemoveUserFromRoleAsync(appUser, roleName);
        }

        public async Task<PaginatedResult<UserDTO>> GetAllUsersAsync(
            Expression<Func<ApplicationUser, bool>>? filter = null,
            string? includeProperties = null,
            bool isAscending = true,
            int pageNumber = 1,
            int pageSize = 10)
        {
            if (pageNumber <= 0)
            {
                throw new ArgumentException("Page number must be greater than 0.", nameof(pageNumber));
            }

            if (pageSize <= 0)
            {
                throw new ArgumentException("Page size must be greater than 0.", nameof(pageSize));
            }

            var cacheKey = CacheKeys.AllUsers(pageNumber, pageSize);

            var cachedUsers = await _cacheService.GetOrCreateAsync(cacheKey, async () =>
            {
                var users = await _unitOfWork.User.GetAllAsync(
                    filter,
                    includeProperties,
                    null,
                    isAscending,
                    pageNumber,
                    pageSize);

                var userDTOs = _mapper.Map<List<UserDTO>>(users.Items);

                return new PaginatedResult<UserDTO>
                {
                    Items = userDTOs,
                    TotalCount = users.TotalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            });

            return cachedUsers;
        }

        public async Task<UserDTO> GetUserByIdAsync(string userId)
        {
            var cacheKey = CacheKeys.UserById(userId);

            var cachedUser = await _cacheService.GetOrCreateAsync(cacheKey, async () =>
            {
                var user = await _unitOfWork.User
                    .GetAsync(u => u.Id == userId) ??
                    throw new KeyNotFoundException($"User with ID '{userId}' not found.");

                var userDTO = _mapper.Map<UserDTO>(user);

                return userDTO;
            });

            return cachedUser;
        }

        public async Task RemoveUserAsync(UserDTO user)
        {
            var appUser = _mapper.Map<ApplicationUser>(user);
            await _unitOfWork.User.RemoveAsync(appUser);
        }
    }
}