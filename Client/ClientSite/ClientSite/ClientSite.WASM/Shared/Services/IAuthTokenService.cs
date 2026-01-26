namespace ClientSite.WASM.Shared.Services
{
    public interface IAuthTokenService
    {
        public Task<bool> RefreshTokenAsync();
        public Task<bool> IsTokenValidAsync();
        public Task<string?> GetValidTokenAsync();
        public Task ClearTokensAsync();
    }
}