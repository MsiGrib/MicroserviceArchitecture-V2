using BLL.Integrations.IdentityMService.DTOs;

namespace BLL.Integrations.IdentityMService.Interfaces
{
    public interface IIdentityServiceIntegration
    {
        public Task<UserSmallInfoDTO?> GetUserSmallInfoAsync(Guid userId, CancellationToken cancellationToken = default);
        public Task<List<UserSmallInfoDTO>?> GetUsersSmallInfoAsync(List<Guid> userIds, CancellationToken cancellationToken = default);
    }
}