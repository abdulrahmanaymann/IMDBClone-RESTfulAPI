using IMDbClone.Core.DTOs.AuthDTOs;
using IMDbClone.Core.DTOs.UserDTOs;
using IMDbClone.Core.Responses;

namespace IMDbClone.Business.Services.IServices
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginDTO);

        Task<APIResponse<UserDTO>> RegisterAsync(RegisterationRequestDTO registerDTO);

        Task<APIResponse<UserProfileDTO>> GetUserProfileAsync(string userId);

        Task<APIResponse<UserProfileDTO>> UpdateUserProfileAsync(string userId, UserProfileDTO userDTO);

        Task<LoginResponseDTO> RefreshTokenAsync(string token, string refreshToken);
    }
}