using ClientSite.WASM.Shared.Storages.Auth.Model;

namespace ClientSite.WASM.Shared.Storages.Auth.Api
{
    public interface IAuthStorageService
    {
        public Task<LocalStorageAuthData?> GetAuthDataAsync();
        public Task SetAuthDataAsync(LocalStorageAuthData authData);
        public Task ClearAuthDataAsync();
        public Task<bool> IsAuthenticatedAsync();
        public Task<string?> GetTokenAsync();
        public Task<string?> GetUserIdAsync();
        public Task<string?> GetUserNameAsync();
    }
}