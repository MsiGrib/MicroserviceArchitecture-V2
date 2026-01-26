using Api.Interfaces;
using Api.Models.MServices.IdentityMService.Endpoints.AuthEndpoints.Requests;
using Api.Models.MServices.IdentityMService.Endpoints.AuthEndpoints.Responses;

namespace ClientSite.WASM.Widgets.Auth.Api
{
    public class SignUpPanelApi(IMicroservicesClient _client)
    {
        public async Task<AuthResponse?> SignUpAsync(string userName, string email, string password, CancellationToken cancellationToken = default)
        {
            var request = new RegisterRequest
            {
                Username = userName,
                Email = email,
                Password = password
            };

            return await _client.Identity.Auth.Register(request, cancellationToken);
        }
    }
}