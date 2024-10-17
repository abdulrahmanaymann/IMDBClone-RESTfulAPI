using IMDbClone.Core.Entities;

namespace IMDbClone.Business.Services.IServices
{
    public interface ITokenService
    {
        string CreateToken(ApplicationUser user);
    }
}
