using System.Net;
using IMDbClone.Business.Services.IServices;
using IMDbClone.Core.DTOs.AuthDTOs;
using IMDbClone.Core.Responses;
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

        [HttpPost("login")]
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

        [HttpPost("Register")]
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
    }
}