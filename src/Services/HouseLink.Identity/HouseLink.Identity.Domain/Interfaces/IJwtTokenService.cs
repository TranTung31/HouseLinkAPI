using HouseLink.Identity.Domain.Entities;

namespace HouseLink.Identity.Domain.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(ApplicationUser user, string identityUserId, IList<string> roles);
        string GenerateRefreshToken();
    }
}
