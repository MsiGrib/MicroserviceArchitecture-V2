using Api.Interfaces;
using Api.Models.MServices.IdentityMService.Endpoints.AuthEndpoints.Requests;
using Api.Models.MServices.IdentityMService.Endpoints.AuthEndpoints.Responses;

namespace ClientSite.WASM.Widgets.Auth.Api
{
    public class SignInPanelApi(IMicroservicesClient _client)
    {
        public async Task<AuthResponse> Login(string login, string password, CancellationToken cancellationToken = default)
        {
            var request = new LoginRequest
            {
                Login = login,
                Password = password,
            };

            return await _client.Identity.Auth.Login(request, cancellationToken);
        }
    }
}