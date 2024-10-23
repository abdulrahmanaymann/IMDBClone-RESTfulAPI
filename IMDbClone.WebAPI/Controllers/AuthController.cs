using System.Net;
using System.Security.Claims;
using IMDbClone.Business.Services.IServices;
using IMDbClone.Core.DTOs.AuthDTOs;
using IMDbClone.Core.DTOs.UserDTOs;
using IMDbClone.Core.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMDbClone.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Logs in a user and returns authentication tokens.
        /// </summary>
        /// <param name="loginDTO">The login data transfer object containing username and password.</param>
        /// <returns>An action result containing the login response.</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(APIResponse<LoginResponseDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse<LoginResponseDTO>), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<LoginResponseDTO>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(APIResponse<LoginResponseDTO>.CreateErrorResponse(
                    new List<string> { "Invalid request data." }, HttpStatusCode.BadRequest));
            }

            var response = await _authService.LoginAsync(loginDTO);
            if (response == null || string.IsNullOrEmpty(response.Token))
            {
                return Unauthorized(APIResponse<LoginResponseDTO>.CreateErrorResponse(
                    new List<string> { "Invalid login credentials." }, HttpStatusCode.Unauthorized));
            }

            return Ok(APIResponse<LoginResponseDTO>.CreateSuccessResponse(response));
        }

        /// <summary>
        /// Registers a new user and returns a response indicating the result of the registration.
        /// </summary>
        /// <param name="registerationDTO">The registration data transfer object containing user details.</param>
        /// <returns>An action result indicating the result of the registration process.</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(APIResponse<LoginResponseDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse<LoginResponseDTO>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse<LoginResponseDTO>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterationRequestDTO registerationDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(APIResponse<LoginResponseDTO>.CreateErrorResponse(
                    new List<string> { "Invalid request data." }, HttpStatusCode.BadRequest));
            }

            try
            {
                var response = await _authService.RegisterAsync(registerationDTO);

                if (!response.IsSuccess)
                {
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<LoginResponseDTO>.CreateErrorResponse(
                    new List<string> { "An error occurred while creating the user. " + ex.InnerException?.Message },
                    HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Retrieves the profile of the logged-in user.
        /// </summary>
        /// <returns>An action result containing the user's profile information.</returns>
        [HttpGet("profile")]
        [Authorize]
        [ProducesResponseType(typeof(APIResponse<UserProfileDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse<UserProfileDTO>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(APIResponse<UserProfileDTO>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UserProfile()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return BadRequest(APIResponse<UserProfileDTO>.CreateErrorResponse(
                    new List<string> { "Invalid user." }, HttpStatusCode.BadRequest));
            }

            var userResponse = await _authService.GetUserProfileAsync(userId);
            if (!userResponse.IsSuccess)
            {
                return NotFound(APIResponse<UserProfileDTO>.CreateErrorResponse(
                    new List<string> { "User not found." }, HttpStatusCode.NotFound));
            }

            return Ok(APIResponse<UserProfileDTO>.CreateSuccessResponse(userResponse.Result));
        }

        /// <summary>
        /// Updates the profile of the logged-in user.
        /// </summary>
        /// <param name="userDTO">The user profile data transfer object containing updated details.</param>
        /// <returns>An action result indicating the result of the update process.</returns>
        [HttpPut("profile")]
        [Authorize]
        [ProducesResponseType(typeof(APIResponse<UserProfileDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse<UserProfileDTO>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(APIResponse<UserProfileDTO>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileDTO userDTO)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return BadRequest(APIResponse<UserProfileDTO>.CreateErrorResponse(
                    new List<string> { "Invalid user." }, HttpStatusCode.BadRequest));
            }

            var userResponse = await _authService.UpdateUserProfileAsync(userId, userDTO);
            if (!userResponse.IsSuccess)
            {
                return NotFound(APIResponse<UserProfileDTO>.CreateErrorResponse(
                    new List<string> { "User not found." }, HttpStatusCode.NotFound));
            }

            return Ok(APIResponse<UserProfileDTO>.CreateSuccessResponse(userResponse.Result));
        }

        /// <summary>
        /// Refreshes the authentication tokens using the provided refresh token.
        /// </summary>
        /// <param name="refreshTokenRequest">The request data transfer object containing the access token and refresh token.</param>
        /// <returns>An action result containing the new tokens or an error message.</returns>
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(APIResponse<LoginResponseDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse<LoginResponseDTO>), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<LoginResponseDTO>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDTO refreshTokenRequest)
        {
            if (string.IsNullOrEmpty(refreshTokenRequest.Token) || string.IsNullOrEmpty(refreshTokenRequest.RefreshToken))
            {
                return BadRequest(APIResponse<LoginResponseDTO>.CreateErrorResponse(
                    new List<string> { "Access token and refresh token are required." }, HttpStatusCode.BadRequest));
            }

            var response = await _authService.RefreshTokenAsync(refreshTokenRequest.Token, refreshTokenRequest.RefreshToken);

            if (string.IsNullOrEmpty(response.Token))
            {
                return Unauthorized(APIResponse<LoginResponseDTO>.CreateErrorResponse(
                    new List<string> { response.Message }, HttpStatusCode.Unauthorized));
            }

            return Ok(APIResponse<LoginResponseDTO>.CreateSuccessResponse(response));
        }
    }
}