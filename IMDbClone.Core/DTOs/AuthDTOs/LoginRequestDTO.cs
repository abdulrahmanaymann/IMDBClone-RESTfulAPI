using System.ComponentModel.DataAnnotations;

namespace IMDbClone.Core.DTOs.AuthDTOs
{
    public class LoginRequestDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}