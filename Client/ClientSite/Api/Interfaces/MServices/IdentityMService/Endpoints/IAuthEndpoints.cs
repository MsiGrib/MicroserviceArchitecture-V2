using Api.Models.MServices.IdentityMService.Endpoints.AuthEndpoints.Requests;
using Api.Models.MServices.IdentityMService.Endpoints.AuthEndpoints.Responses;

namespace Api.Interfaces.MServices.IdentityMService.Endpoints
{
    public interface IAuthEndpoints
    {
        public Task<AuthResponse> Register(RegisterRequest request, CancellationToken cancellationToken = default);
        public Task<AuthResponse> Login(LoginRequest request, CancellationToken cancellationToken = default);
        public Task<AuthResponse> Refresh(RefreshTokenRequest request, CancellationToken cancellationToken = default);
        public Task LogOut(RefreshTokenRequest request, CancellationToken cancellationToken = default);
    }
}