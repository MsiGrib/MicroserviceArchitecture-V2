using Api.Models.MServices.IdentityMService.Endpoints.UserEndpoints.Responses;

namespace Api.Interfaces.MServices.IdentityMService.Endpoints
{
    public interface IUserEndpoints
    {
        public Task<UserDTO> GetProfile(string token, CancellationToken cancellationToken = default);
    }
}