using BLL.DTOs.User.DTO;
using BLL.Services.Auth;
using BLL.Services.Interfaces.User;
using DAL.Entities;
using DAL.Repositories.Interfaces.User;
using Microsoft.Extensions.Logging;

namespace BLL.Services.User
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthService> _logger;

        public UserService(IUserRepository userRepository, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<UserDTO?> GetCurrentUser(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                return null;

            return new UserDTO
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                LastLoginAt = user.LastLoginAt,
            };
        }

        public async Task<UserSmallInfoDTO?> GetUserSmallInfo(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                return null;

            return new UserSmallInfoDTO
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
            };
        }

        public async Task<List<UserSmallInfoDTO>?> GetBatchUserSmallInfo(List<Guid> userIds)
        {
            var users = await _userRepository.GetByIdsAsync(userIds);

            if (users == null || users.Count == 0)
                return null;

            return users.Select(x => new UserSmallInfoDTO
            {
                Id = x.Id,
                Email = x.Email,
                Username = x.Username,
            }).ToList();
        }
    }
}