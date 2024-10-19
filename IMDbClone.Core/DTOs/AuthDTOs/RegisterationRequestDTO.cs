using System.ComponentModel.DataAnnotations;
using IMDbClone.Core.Validation;

namespace IMDbClone.Core.DTOs.AuthDTOs
{
    public class RegisterationRequestDTO
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [FullName]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}