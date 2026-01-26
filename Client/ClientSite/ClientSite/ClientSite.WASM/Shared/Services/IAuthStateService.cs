using ClientSite.WASM.Shared.Storages.Model;

namespace ClientSite.WASM.Shared.Services
{
    public interface IAuthStateService
    {
        public event Action? OnAuthStateChanged;
        public Task<bool> CheckAuthenticationAsync();
        public Task SetAuthenticatedAsync(ClientSettings settings);
        public Task LogoutAsync();
        public bool IsAuthenticated();
    }
}