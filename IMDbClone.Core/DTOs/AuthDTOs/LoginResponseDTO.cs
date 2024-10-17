using IMDbClone.Core.DTOs.UserDTOs;

namespace IMDbClone.Core.DTOs.AuthDTOs
{
    public class LoginResponseDTO
    {
        public UserDTO User { get; set; }

        public string Token { get; set; }

        public string Message { get; set; }
    }
}