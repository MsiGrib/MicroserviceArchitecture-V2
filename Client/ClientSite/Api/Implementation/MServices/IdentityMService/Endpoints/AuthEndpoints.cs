using Api.Interfaces;
using Api.Interfaces.MServices.IdentityMService.Endpoints;
using Api.Models.MServices.IdentityMService.Endpoints.AuthEndpoints.Requests;
using Api.Models.MServices.IdentityMService.Endpoints.AuthEndpoints.Responses;
using RestSharp;

namespace Api.Implementation.MServices.IdentityMService.Endpoints
{
    internal class AuthEndpoints : BaseEndPoint, IAuthEndpoints
    {
        public AuthEndpoints(IMicroservicesClient client, string basePath)
            : base(client, "Identity", basePath) { }

        public Task<AuthResponse> Register(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            var restRequest = new RestRequest(BuildUrl("/register"), Method.Post);

            restRequest.AddJsonBody(request);

            return ExecuteAsync<AuthResponse>(restRequest, ctn: cancellationToken);
        }

        public Task<AuthResponse> Login(LoginRequest request, CancellationToken cancellationToken = default)
        {
            var restRequest = new RestRequest(BuildUrl("/login"), Method.Post);

            restRequest.AddJsonBody(request);

            return ExecuteAsync<AuthResponse>(restRequest, ctn: cancellationToken);
        }

        public Task<AuthResponse> Refresh(RefreshTokenRequest request, CancellationToken cancellationToken = default)
        {
            var restRequest = new RestRequest(BuildUrl("/refresh"), Method.Post);

            restRequest.AddJsonBody(request);

            return ExecuteAsync<AuthResponse>(restRequest, ctn: cancellationToken);
        }

        public Task LogOut(RefreshTokenRequest request, string token, CancellationToken cancellationToken = default)
        {
            var restRequest = new RestRequest(BuildUrl("/logout"), Method.Post);

            restRequest.AddJsonBody(request);

            return ExecuteAsync(restRequest, token, ctn: cancellationToken);
        }
    }
}