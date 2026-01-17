using BLL.DTOs.Auth.Requests;
using BLL.DTOs.Auth.Responses;

namespace BLL.Services.Interfaces.Auth
{
    public interface IAuthService
    {
        public Task<AuthResponse> RegisterAsync(RegisterRequest request);
        public Task<AuthResponse> LoginAsync(LoginRequest request);
        public Task<AuthResponse> RefreshTokenAsync(string refreshToken);
        public Task RevokeTokenAsync(string refreshToken, Guid userId);
        public Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
        public Task LogoutAsync(string refreshToken, Guid userId);
    }
}