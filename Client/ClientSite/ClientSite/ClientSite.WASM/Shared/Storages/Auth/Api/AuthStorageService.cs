using ClientSite.WASM.Shared.Storages.Auth.Model;

namespace ClientSite.WASM.Shared.Storages.Auth.Api
{
    public class AuthStorageService : IAuthStorageService
    {
        #region Private Fields

        private const string AuthStorageKey = "auth_data";
        private readonly ILocalStorageService _localStorage;
        private LocalStorageAuthData? _cachedAuthData = null;

        #endregion

        #region Ctors

        public AuthStorageService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        #endregion

        #region Public methods

        public async Task<LocalStorageAuthData?> GetAuthDataAsync()
        {
            if (_cachedAuthData != null)
                return _cachedAuthData;

            _cachedAuthData = await _localStorage.GetItemAsync<LocalStorageAuthData>(AuthStorageKey);
            return _cachedAuthData;
        }

        public async Task SetAuthDataAsync(LocalStorageAuthData authData)
        {
            _cachedAuthData = authData;
            await _localStorage.SetItemAsync(AuthStorageKey, authData);
        }

        public async Task ClearAuthDataAsync()
        {
            _cachedAuthData = null;
            await _localStorage.RemoveItemAsync(AuthStorageKey);
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var authData = await GetAuthDataAsync();
            return authData?.IsAuthenticated ?? false;
        }

        public async Task<string?> GetTokenAsync()
        {
            var authData = await GetAuthDataAsync();
            return authData?.AccessToken;
        }

        public async Task<string?> GetUserIdAsync()
        {
            var authData = await GetAuthDataAsync();
            return authData?.UserId;
        }

        public async Task<string?> GetUserNameAsync()
        {
            var authData = await GetAuthDataAsync();
            return authData?.UserName;
        }

        #endregion
    }
}