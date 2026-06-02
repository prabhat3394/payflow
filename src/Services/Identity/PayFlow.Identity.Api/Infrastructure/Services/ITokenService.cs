using PayFlow.Identity.Api.Domain.Entities;

namespace PayFlow.Identity.Api.Infrastructure.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(AppUser user);
        string GenerateRefreshToken();

    }
}
