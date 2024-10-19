using IMDbClone.Core.DTOs.UserDTOs;

namespace IMDbClone.Core.DTOs.AuthDTOs
{
    public class LoginResponseDTO
    {
        public UserDTO User { get; set; } = default!;

        public string Token { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}