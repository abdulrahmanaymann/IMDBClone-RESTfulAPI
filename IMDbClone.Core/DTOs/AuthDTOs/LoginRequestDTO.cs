using System.ComponentModel.DataAnnotations;

namespace IMDbClone.Core.DTOs.AuthDTOs
{
    public class LoginRequestDTO
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}