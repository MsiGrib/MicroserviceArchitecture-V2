using ClientSite.WASM.Shared.Storages.Lib;
using ClientSite.WASM.Shared.Storages.Model;

namespace ClientSite.WASM.Shared.Services
{
    public class AuthStateService : IAuthStateService
    {
        #region Private Fields

        private readonly IClientStorage _clientStorage;
        private readonly IAuthTokenService _authTokenService;
        private bool _isAuthenticated = false;

        #endregion

        #region Public Fields

        public event Action? OnAuthStateChanged;

        #endregion

        public AuthStateService(IClientStorage clientStorage, IAuthTokenService authTokenService)
        {
            _clientStorage = clientStorage;
            _authTokenService = authTokenService;
        }

        #region Public methods

        public async Task<bool> CheckAuthenticationAsync()
        {
            _isAuthenticated = await _authTokenService.IsTokenValidAsync();
            return _isAuthenticated;
        }

        public async Task<bool> RefreshTokenAsync()
        {
            var success = await _authTokenService.RefreshTokenAsync();
            if (success)
            {
                _isAuthenticated = true;
                NotifyStateChanged();
            }
            return success;
        }

        public async Task SetAuthenticatedAsync(ClientSettings settings)
        {
            await _clientStorage.ClearClientSettingsAsync();
            await _clientStorage.SetClientSettingsAsync(settings);
            _isAuthenticated = true;
            NotifyStateChanged();
        }

        public async Task LogoutAsync()
        {
            await _authTokenService.ClearTokensAsync();
            _isAuthenticated = false;
            NotifyStateChanged();
        }

        public bool IsAuthenticated()
            => _isAuthenticated;

        private void NotifyStateChanged()
            => OnAuthStateChanged?.Invoke();

        #endregion
    }
}