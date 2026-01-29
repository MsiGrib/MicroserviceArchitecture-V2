using BLL.DTOs.User.DTO;

namespace BLL.Services.Interfaces.User
{
    public interface IUserService
    {
        public Task<UserDTO?> GetCurrentUser(Guid userId);
    }
}