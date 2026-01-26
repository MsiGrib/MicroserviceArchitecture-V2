using Api.Interfaces;
using Api.Interfaces.MServices.IdentityMService.Endpoints;
using Api.Models.MServices.IdentityMService.Endpoints.UserEndpoints.Responses;
using RestSharp;

namespace Api.Implementation.MServices.IdentityMService.Endpoints
{
    internal class UserEndpoints : BaseEndPoint, IUserEndpoints
    {
        public UserEndpoints(IMicroservicesClient client, string basePath)
            : base(client, "Identity", basePath) { }

        public Task<UserDTO> GetProfile(string token, CancellationToken cancellationToken = default)
        {
            var restRequest = new RestRequest(BuildUrl("/me"), Method.Get);
            return ExecuteAsync<UserDTO>(restRequest, token, ctn: cancellationToken);
        }
    }
}