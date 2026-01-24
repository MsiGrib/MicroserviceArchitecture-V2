using BLL.DTOs.Auth.Requests;
using BLL.DTOs.Auth.Responses;
using BLL.Services.Interfaces.Auth;
using Common.Models;
using DAL.Repositories.Interfaces.User;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BLL.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IDistributedCache _redisCache;
        private readonly IOptions<JwtSettings> _jwtSettings;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher,
            IDistributedCache redisCache, IOptions<JwtSettings> jwtSettings, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _redisCache = redisCache;
            _jwtSettings = jwtSettings;
            _logger = logger;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            if (await _userRepository.ExistsByEmailAsync(request.Email))
                throw new Exception("Email already exists");

            if (await _userRepository.ExistsByUsernameAsync(request.Username))
                throw new Exception("Username already exists");

            var user = new DAL.Entities.User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Username = request.Username,
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                LastLoginAt = DateTime.UtcNow,
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            var tokens = await GenerateTokensAsync(user);

            _logger.LogInformation("User registered: {Email}", user.Email);

            return tokens;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Login)
                     ?? await _userRepository.GetByUsernameAsync(request.Login);

            if (user == null || !user.IsActive)
                throw new Exception("Invalid login credentials");

            if (!_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
                throw new Exception("Invalid login credentials");

            user.LastLoginAt = DateTime.UtcNow;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            var tokens = await GenerateTokensAsync(user);

            _logger.LogInformation("User logged in: {Email}", user.Email);

            return tokens;
        }

        public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
        {
            var tokenKey = $"refresh:{refreshToken}";
            var userIdString = await _redisCache.GetStringAsync(tokenKey);

            if (string.IsNullOrEmpty(userIdString))
                throw new Exception("Invalid refresh token");

            if (!Guid.TryParse(userIdString, out var userId))
                throw new Exception("Invalid refresh token");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || !user.IsActive)
                throw new Exception("User not found or inactive");

            await _redisCache.RemoveAsync(tokenKey);

            var tokens = await GenerateTokensAsync(user);

            _logger.LogInformation("Token refreshed for user: {Email}", user.Email);

            return tokens;
        }

        public async Task RevokeTokenAsync(string refreshToken, Guid userId)
        {
            var tokenKey = $"refresh:{refreshToken}";
            var storedUserId = await _redisCache.GetStringAsync(tokenKey);

            if (!string.IsNullOrEmpty(storedUserId) && storedUserId == userId.ToString())
            {
                await _redisCache.RemoveAsync(tokenKey);
                _logger.LogInformation("Token revoked for user: {UserId}", userId);
            }
        }

        public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            if (!_passwordHasher.VerifyPassword(user.PasswordHash, request.CurrentPassword))
                throw new Exception("Current password is incorrect");

            user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task LogoutAsync(string refreshToken, Guid userId)
        {
            var tokenKey = $"refresh:{refreshToken}";
            await _redisCache.RemoveAsync(tokenKey);

            _logger.LogInformation("User logged out: {UserId}", userId);
        }

        private async Task<AuthResponse> GenerateTokensAsync(DAL.Entities.User user)
        {
            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            var tokenKey = $"refresh:{refreshToken}";
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(_jwtSettings.Value.RefreshTokenExpirationDays)
            };

            await _redisCache.SetStringAsync(tokenKey, user.Id.ToString(), cacheOptions);

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = _jwtSettings.Value.AccessTokenExpirationMinutes * 60,
                UserId = user.Id,
                Email = user.Email,
                Username = user.Username,
            };
        }

        private string GenerateAccessToken(DAL.Entities.User user)
        {
            var jwtSettings = _jwtSettings.Value;

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("is_active", user.IsActive.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtSettings.AccessTokenExpirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();

            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }
    }
}