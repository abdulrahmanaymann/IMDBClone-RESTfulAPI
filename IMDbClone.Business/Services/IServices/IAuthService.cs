using IMDbClone.Core.DTOs.AuthDTOs;
using IMDbClone.Core.DTOs.UserDTOs;

namespace IMDbClone.Business.Services.IServices
{
    public interface IAuthService
    {
        Task<UserDTO> RegisterAsync(RegisterationRequestDTO registerDTO);
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginDTO);
    }
}
