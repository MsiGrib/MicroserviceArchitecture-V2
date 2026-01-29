using System.Security.Claims;

namespace ApiGateway.Services.Interfaces
{
    public interface IJwtService
    {
        public Task<bool> ValidateTokenAsync(string token);
        public Task<ClaimsPrincipal?> GetPrincipalFromTokenAsync(string token);
        public string CreateHeaderSignature(string userId, IEnumerable<string> roles);
    }
}