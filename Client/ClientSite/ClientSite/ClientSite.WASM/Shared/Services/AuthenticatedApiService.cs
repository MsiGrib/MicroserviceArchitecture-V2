using System.Net;

namespace ClientSite.WASM.Shared.Services
{
    public class AuthenticatedApiService : IAuthenticatedApiService
    {
        private readonly IAuthTokenService _authTokenService;
        private readonly IAuthStateService _authStateService;

        public AuthenticatedApiService(IAuthTokenService authTokenService, IAuthStateService authStateService)
        {
            _authTokenService = authTokenService;
            _authStateService = authStateService;
        }

        public async Task<T> ExecuteWithTokenRefreshAsync<T>(Func<string, Task<T>> apiCall)
        {
            var maxRetryCount = 1;

            for (int attempt = 0; attempt <= maxRetryCount; attempt++)
            {
                try
                {
                    var token = await _authTokenService.GetValidTokenAsync();
                    if (string.IsNullOrEmpty(token))
                        throw new UnauthorizedAccessException("No valid token available");

                    return await apiCall(token);
                }
                catch (UnauthorizedAccessException) when (attempt < maxRetryCount)
                {
                    var refreshSuccess = await _authTokenService.RefreshTokenAsync();
                    if (!refreshSuccess)
                    {
                        await _authStateService.LogoutAsync();
                        throw;
                    }
                }
                catch (HttpRequestException ex) when (
                    ex.StatusCode == HttpStatusCode.Unauthorized &&
                    attempt < maxRetryCount)
                {
                    var refreshSuccess = await _authTokenService.RefreshTokenAsync();
                    if (!refreshSuccess)
                    {
                        await _authStateService.LogoutAsync();
                        throw;
                    }
                }
            }

            throw new UnauthorizedAccessException("Authentication failed after token refresh");
        }

        public async Task ExecuteWithTokenRefreshAsync(Func<string, Task> apiCall)
        {
            await ExecuteWithTokenRefreshAsync<object?>(async token =>
            {
                await apiCall(token);
                return null;
            });
        }
    }
}