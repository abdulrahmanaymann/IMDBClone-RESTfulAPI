using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IMDbClone.Business.Services.IServices;
using IMDbClone.Core.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace IMDbClone.Business.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;

        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]));
        }

        public string CreateToken(ApplicationUser user)
        {
            // step 1: create claims
            List<Claim> claims = new()
            {
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.UniqueName, user.UserName),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // step 2: create credentials
            SigningCredentials creds = new(_key, SecurityAlgorithms.HmacSha256Signature);

            // step 3: create token descriptor
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = creds,
                Issuer = _config["JwtSettings:Issuer"],
                Audience = _config["JwtSettings:Audience"]
            };

            // step 4: create token
            JwtSecurityTokenHandler tokenHandler = new();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            // step 5: return token
            return tokenHandler.WriteToken(token);
        }
    }
}