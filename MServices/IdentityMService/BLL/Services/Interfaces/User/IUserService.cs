using BLL.DTOs.User.Responses;

namespace BLL.Services.Interfaces.User
{
    public interface IUserService
    {
        public Task<UserDTO?> GetCurrentUser(Guid userId);
    }
}