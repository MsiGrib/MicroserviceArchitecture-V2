using BLL.DTOs.User.DTO;

namespace BLL.Services.Interfaces.User
{
    public interface IUserService
    {
        public Task<UserDTO?> GetCurrentUser(Guid userId);
        public Task<UserSmallInfoDTO?> GetUserSmallInfo(Guid userId);
        public Task<List<UserSmallInfoDTO>?> GetBatchUserSmallInfo(List<Guid> userIds);
    }
}