using System.Security.Claims;

namespace smarth_health.WebApi.Services
{
    public interface IIdentityService
    {
        Task<string> GetCurrentUserIdAsync(ClaimsPrincipal user);
    }
}
