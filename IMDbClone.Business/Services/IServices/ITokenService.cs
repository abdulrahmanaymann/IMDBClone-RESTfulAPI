using System.Security.Claims;
using IMDbClone.Core.Models;

namespace IMDbClone.Business.Services.IServices
{
    public interface ITokenService
    {
        string CreateToken(ApplicationUser user);

        string CreateRefreshToken();

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
