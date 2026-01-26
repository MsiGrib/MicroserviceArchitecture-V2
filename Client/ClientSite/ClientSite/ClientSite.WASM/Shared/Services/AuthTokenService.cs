using Api.Interfaces;
using Api.Models.MServices.IdentityMService.Endpoints.AuthEndpoints.Requests;
using ClientSite.WASM.Shared.Storages.Lib;
using ClientSite.WASM.Shared.Storages.Model;

namespace ClientSite.WASM.Shared.Services
{
    public class AuthTokenService : IAuthTokenService
    {
        private readonly IClientStorage _clientStorage;
        private readonly IMicroservicesClient _microservicesClient;
        private readonly SemaphoreSlim _refreshLock = new(1, 1);
        private DateTime? _lastRefreshAttempt;

        public AuthTokenService(IClientStorage clientStorage, IMicroservicesClient microservicesClient)
        {
            _clientStorage = clientStorage;
            _microservicesClient = microservicesClient;
        }

        public async Task<string?> GetValidTokenAsync()
        {
            var settings = await _clientStorage.GetClientSettingsAsync();

            if (settings == null ||
                string.IsNullOrWhiteSpace(settings.AccessToken) ||
                string.IsNullOrWhiteSpace(settings.RefreshToken))
            {
                return null;
            }

            return settings.AccessToken;
        }

        public async Task<bool> IsTokenValidAsync()
            => await GetValidTokenAsync() != null;

        public async Task<bool> RefreshTokenAsync()
        {
            if (!await _refreshLock.WaitAsync(TimeSpan.FromSeconds(5)))
                return false;

            try
            {
                if (_lastRefreshAttempt.HasValue && (DateTime.UtcNow - _lastRefreshAttempt.Value).TotalSeconds < 5)
                    return false;

                _lastRefreshAttempt = DateTime.UtcNow;

                var settings = await _clientStorage.GetClientSettingsAsync();
                if (settings == null || string.IsNullOrWhiteSpace(settings.RefreshToken))
                    return false;

                try
                {
                    var authApi = _microservicesClient.Identity.Auth;
                    var response = await authApi.Refresh(new RefreshTokenRequest
                    {
                        RefreshToken = settings.RefreshToken
                    });

                    if (response != null)
                    {
                        var newSettings = new ClientSettings
                        {
                            AccessToken = response.AccessToken,
                            RefreshToken = response.RefreshToken,
                            ExpiresIn = response.ExpiresIn,
                            UserId = response.UserId,
                            Email = response.Email,
                            Username = response.Username
                        };

                        await _clientStorage.SetClientSettingsAsync(newSettings);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    await ClearTokensAsync();
                    Console.WriteLine($"Failed to refresh token: {ex.Message}");
                }

                return false;
            }
            finally
            {
                _refreshLock.Release();
            }
        }

        public async Task ClearTokensAsync()
            => await _clientStorage.ClearClientSettingsAsync();
    }
}