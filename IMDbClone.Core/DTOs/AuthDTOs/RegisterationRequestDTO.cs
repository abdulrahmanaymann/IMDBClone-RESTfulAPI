using System.ComponentModel.DataAnnotations;
using IMDbClone.Core.Validation;

namespace IMDbClone.Core.DTOs.AuthDTOs
{
    public class RegisterationRequestDTO
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [FullName]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}