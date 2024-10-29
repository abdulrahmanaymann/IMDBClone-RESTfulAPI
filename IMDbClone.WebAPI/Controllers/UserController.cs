using System.Linq.Expressions;
using System.Net;
using IMDbClone.Business.Services.IServices;
using IMDbClone.Core.DTOs.UserDTOs;
using IMDbClone.Core.Models;
using IMDbClone.Core.Responses;
using IMDbClone.Core.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMDbClone.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Retrieves all available roles in the system.
        /// </summary>
        /// <returns>An API response containing a list of roles.</returns>
        /// <response code="200">Returns a list of roles.</response>
        /// <response code="500">If there is a server error.</response>
        [HttpGet("roles")]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<string>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                var roles = await _userService.GetAllRolesAsync();
                var response = APIResponse<IEnumerable<string>>.CreateSuccessResponse(roles);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = APIResponse<IEnumerable<string>>
                    .CreateErrorResponse(new List<string> { ex.Message });
                return StatusCode((int)response.StatusCode, response);
            }
        }

        /// <summary>
        /// Retrieves the roles assigned to a specific user by their ID.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>An API response containing the user's roles.</returns>
        /// <response code="200">Returns the user's roles.</response>
        /// <response code="404">If the user is not found.</response>
        /// <response code="400">If the user ID is null or empty.</response>
        /// <response code="500">If there is a server error.</response>
        [HttpGet("{userId}/roles")]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<string>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                var response = APIResponse<IEnumerable<string>>.CreateErrorResponse(
                    new List<string> { "User ID must not be null or empty." }, HttpStatusCode.BadRequest);
                return BadRequest(response);
            }

            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    var response = APIResponse<IEnumerable<string>>
                        .CreateErrorResponse(new List<string> { "User not found." }, HttpStatusCode.NotFound);
                    return NotFound(response);
                }

                var roles = await _userService.GetUserRolesAsync(user);
                var successResponse = APIResponse<IEnumerable<string>>.CreateSuccessResponse(roles);
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                var response = APIResponse<IEnumerable<string>>.CreateErrorResponse(new List<string> { ex.Message });
                return StatusCode((int)response.StatusCode, response);
            }
        }

        /// <summary>
        /// Assigns a role to a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="roleName">The name of the role to assign.</param>
        /// <returns>An empty response if successful.</returns>
        /// <response code="204">If the role is successfully assigned.</response>
        /// <response code="404">If the user is not found.</response>
        /// <response code="400">If the user ID or role name is null or empty.</response>
        /// <response code="500">If there is a server error.</response>
        [HttpPost("{userId}/roles")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddUserToRole(string userId, [FromBody] string roleName)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roleName))
            {
                var response = APIResponse<string>.CreateErrorResponse(
                    new List<string> { "User ID and role name must not be null or empty." }, HttpStatusCode.BadRequest);
                return BadRequest(response);
            }

            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    var response = APIResponse<string>.CreateErrorResponse(
                        new List<string> { "User not found." }, HttpStatusCode.NotFound);
                    return NotFound(response);
                }

                await _userService.AddUserToRoleAsync(user, roleName);
                return NoContent();
            }
            catch (Exception ex)
            {
                var response = APIResponse<string>.CreateErrorResponse(new List<string> { ex.Message });
                return StatusCode((int)response.StatusCode, response);
            }
        }

        /// <summary>
        /// Updates the roles assigned to a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="roles">The new roles to assign.</param>
        /// <returns>An empty response if successful.</returns>
        /// <response code="204">If the roles are successfully updated.</response>
        /// <response code="404">If the user is not found.</response>
        /// <response code="400">If the user ID is null or empty, or if no roles are provided.</response>
        /// <response code="500">If there is a server error.</response>
        [HttpPut("{userId}/roles")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateRoles(string userId, [FromBody] IEnumerable<string> roles)
        {
            if (string.IsNullOrEmpty(userId) || roles == null || !roles.Any())
            {
                var response = APIResponse<string>.CreateErrorResponse(
                    new List<string> { "User ID must not be null or empty, and roles must be provided." },
                    HttpStatusCode.BadRequest);
                return BadRequest(response);
            }

            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    var response = APIResponse<string>.CreateErrorResponse(
                        new List<string> { "User not found." }, HttpStatusCode.NotFound);
                    return NotFound(response);
                }

                await _userService.UpdateRolesAsync(user, roles);
                return NoContent();
            }
            catch (Exception ex)
            {
                var response = APIResponse<string>.CreateErrorResponse(new List<string> { ex.Message });
                return StatusCode((int)response.StatusCode, response);
            }
        }

        /// <summary>
        /// Removes a role from a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="roleName">The name of the role to remove.</param>
        /// <returns>An empty response if successful.</returns>
        /// <response code="204">If the role is successfully removed.</response>
        /// <response code="404">If the user is not found.</response>
        /// <response code="400">If the user ID or role name is null or empty.</response>
        /// <response code="500">If there is a server error.</response>
        [HttpDelete("{userId}/roles")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveUserFromRole(string userId, [FromBody] string roleName)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roleName))
            {
                var response = APIResponse<string>.CreateErrorResponse(
                    new List<string> { "User ID and role name must not be null or empty." }, HttpStatusCode.BadRequest);
                return BadRequest(response);
            }

            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    var response = APIResponse<string>.CreateErrorResponse(
                        new List<string> { "User not found." }, HttpStatusCode.NotFound);
                    return NotFound(response);
                }

                await _userService.RemoveUserFromRoleAsync(user, roleName);
                return NoContent();
            }
            catch (Exception ex)
            {
                var response = APIResponse<string>.CreateErrorResponse(new List<string> { ex.Message });
                return StatusCode((int)response.StatusCode, response);
            }
        }

        /// <summary>
        /// Retrieves all users with optional filtering and sorting by name.
        /// </summary>
        /// <param name="filter">Optional filter to apply to the users' names.</param>
        /// <param name="isAscending">Indicates whether the sorting should be in ascending order.</param>
        /// <param name="pageNumber">The page number to retrieve (default is 1).</param>
        /// <param name="pageSize">The number of users per page (default is 10).</param>
        /// <returns>A paginated result containing the list of users.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(APIResponse<PaginatedResult<UserDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllUsers(
             [FromQuery] string? filter = null,
             [FromQuery] bool isAscending = true,
             [FromQuery] int pageNumber = 1,
             [FromQuery] int pageSize = 10)
        {
            if (pageNumber <= 0)
            {
                return BadRequest(APIResponse<PaginatedResult<UserDTO>>.CreateErrorResponse(
                    new List<string> { "Page number must be greater than 0." }, HttpStatusCode.BadRequest));
            }

            if (pageSize <= 0)
            {
                return BadRequest(APIResponse<PaginatedResult<UserDTO>>.CreateErrorResponse(
                    new List<string> { "Page size must be greater than 0." }, HttpStatusCode.BadRequest));
            }

            try
            {
                Expression<Func<ApplicationUser, bool>> userFilter = user => string.IsNullOrEmpty(filter) || user.Name.Contains(filter);

                var users = await _userService.GetAllUsersAsync(
                    filter: userFilter,
                    isAscending: isAscending,
                    pageNumber: pageNumber,
                    pageSize: pageSize);

                var response = APIResponse<PaginatedResult<UserDTO>>.CreateSuccessResponse(users);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = APIResponse<PaginatedResult<UserDTO>>
                        .CreateErrorResponse(new List<string> { ex.Message });
                return StatusCode((int)response.StatusCode, response);
            }
        }

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <returns>The user details.</returns>
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(APIResponse<UserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserById(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                var response = APIResponse<UserDTO>.CreateErrorResponse(
                    new List<string> { "User ID must not be null or empty." }, HttpStatusCode.BadRequest);
                return BadRequest(response);
            }

            try
            {
                var user = await _userService.GetUserByIdAsync(userId);

                var response = APIResponse<UserDTO>.CreateSuccessResponse(user);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                var response = APIResponse<UserDTO>
                        .CreateErrorResponse(new List<string> { ex.Message }, HttpStatusCode.NotFound);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = APIResponse<UserDTO>.CreateErrorResponse(new List<string> { ex.Message });
                return StatusCode((int)response.StatusCode, response);
            }
        }

        /// <summary>
        /// Removes a user by their unique identifier.
        /// </summary>
        /// <param name="userId">The ID of the user to remove.</param>
        /// <returns>No content if successful.</returns>
        [HttpDelete("{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                var response = APIResponse<string>.CreateErrorResponse(
                    new List<string> { "User ID must not be null or empty." }, HttpStatusCode.BadRequest);
                return BadRequest(response);
            }

            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                await _userService.RemoveUserAsync(user);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                var response = APIResponse<string>
                        .CreateErrorResponse(new List<string> { ex.Message }, HttpStatusCode.NotFound);
                return NotFound(response);
            }
            catch (Exception ex)
            {
                var response = APIResponse<string>.CreateErrorResponse(new List<string> { ex.Message });
                return StatusCode((int)response.StatusCode, response);
            }
        }
    }
}