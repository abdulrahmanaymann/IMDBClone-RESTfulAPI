using System.Text.Json.Serialization;
using IMDbClone.Core.DTOs.UserDTOs;
using Newtonsoft.Json.Converters;

namespace IMDbClone.Core.DTOs.AuthDTOs
{
    public class LoginResponseDTO
    {
        public UserDTO User { get; set; } = default!;

        public string Token { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string? RefreshToken { get; set; }

        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime? RefreshTokenExpiryTime { get; set; }

    }
}