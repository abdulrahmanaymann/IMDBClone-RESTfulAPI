using System.Net;
using System.Security.Claims;
using AutoMapper;
using IMDbClone.Business.Services.IServices;
using IMDbClone.Common;
using IMDbClone.Core.DTOs.AuthDTOs;
using IMDbClone.Core.DTOs.UserDTOs;
using IMDbClone.Core.Entities;
using IMDbClone.Core.Responses;
using IMDbClone.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace IMDbClone.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly ITokenService _tokenService;

        private readonly IMapper _mapper;

        private readonly int _refreshTokenExpiryDays;


        public AuthService(
            IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ITokenService tokenService,
            IMapper mapper,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _mapper = mapper;
            _refreshTokenExpiryDays = int.Parse(configuration["JwtSettings:RefreshTokenExpiryDays"]!);
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
            {
                return new LoginResponseDTO()
                {
                    Token = "",
                    RefreshToken = "",
                    User = null,
                    Message = "Invalid username or password."
                };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);
            if (!result.Succeeded)
            {
                return new LoginResponseDTO()
                {
                    Token = "",
                    RefreshToken = "",
                    User = null,
                    Message = "Invalid username or password."
                };
            }

            var token = _tokenService.CreateToken(user);
            var refreshToken = _tokenService.CreateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

            await _userManager.UpdateAsync(user);

            return new LoginResponseDTO()
            {
                Token = token,
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = user.RefreshTokenExpiryTime,
                User = _mapper.Map<UserDTO>(user),
                Message = "Login successful."
            };
        }


        public async Task<APIResponse<UserDTO>> RegisterAsync(RegisterationRequestDTO registerDTO)
        {
            bool isUserUnique = await _unitOfWork.User.IsUniqueUser(registerDTO.UserName);
            if (!isUserUnique)
            {
                return APIResponse<UserDTO>.CreateErrorResponse(
                    new List<string> { "User already exists! Please try again with a different username." },
                    HttpStatusCode.BadRequest);
            }

            // Step 1: create a new user
            var user = _mapper.Map<ApplicationUser>(registerDTO);
            try
            {
                // Step 2: create the user
                var result = await _userManager.CreateAsync(user, registerDTO.Password);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return APIResponse<UserDTO>.CreateErrorResponse(errors, HttpStatusCode.BadRequest);
                }

                // Step 3: add user to the default role
                await _userManager.AddToRoleAsync(user, Roles.User);

                var userDTO = _mapper.Map<UserDTO>(user);
                return APIResponse<UserDTO>.CreateSuccessResponse(userDTO);
            }
            catch (Exception ex)
            {
                return APIResponse<UserDTO>.CreateErrorResponse(
                    new List<string> { "An error occurred while creating the user. Please try again later." },
                    HttpStatusCode.InternalServerError);
            }
        }

        public async Task<APIResponse<UserProfileDTO>> GetUserProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return APIResponse<UserProfileDTO>.CreateErrorResponse(
                    new List<string> { "User not found." },
                    HttpStatusCode.NotFound);
            }

            var userDTO = _mapper.Map<UserProfileDTO>(user);

            return APIResponse<UserProfileDTO>.CreateSuccessResponse(userDTO);
        }

        public async Task<APIResponse<UserProfileDTO>> UpdateUserProfileAsync(string userId, UserProfileDTO userDTO)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return APIResponse<UserProfileDTO>.CreateErrorResponse(
                    new List<string> { "User not found." },
                    HttpStatusCode.NotFound);
            }

            _mapper.Map(userDTO, user);

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return APIResponse<UserProfileDTO>.CreateErrorResponse(errors, HttpStatusCode.BadRequest);
            }

            return APIResponse<UserProfileDTO>.CreateSuccessResponse(userDTO);
        }

        public async Task<LoginResponseDTO> RefreshTokenAsync(string token, string refreshToken)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(token);
            var userId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return new LoginResponseDTO()
                {
                    Token = "",
                    RefreshToken = "",
                    User = null,
                    Message = "Invalid refresh token or token expired."
                };
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return new LoginResponseDTO()
                {
                    Token = "",
                    RefreshToken = "",
                    User = null,
                    Message = "Invalid refresh token or token expired."
                };
            }

            var newToken = _tokenService.CreateToken(user);
            var newRefreshToken = _tokenService.CreateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

            await _userManager.UpdateAsync(user);

            return new LoginResponseDTO()
            {
                Token = newToken,
                RefreshToken = newRefreshToken,
                RefreshTokenExpiryTime = user.RefreshTokenExpiryTime,
                User = _mapper.Map<UserDTO>(user),
                Message = "Token refreshed successfully."
            };
        }
    }
}