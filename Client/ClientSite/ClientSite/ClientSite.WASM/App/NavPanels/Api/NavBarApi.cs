using Api.Interfaces;
using Api.Models.MServices.IdentityMService.Endpoints.AuthEndpoints.Requests;

namespace ClientSite.WASM.App.NavPanels.Api
{
    public class NavBarApi(IMicroservicesClient _client)
    {
        public async Task LogOut(string refreshToken, string token, CancellationToken cancellationToken = default)
        {
            var request = new RefreshTokenRequest
            {
                RefreshToken = refreshToken,
            };

            await _client.Identity.Auth.LogOut(request, token, cancellationToken);
        }
    }
}