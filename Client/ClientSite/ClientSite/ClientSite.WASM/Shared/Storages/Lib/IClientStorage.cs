using ClientSite.WASM.Shared.Storages.Model;

namespace ClientSite.WASM.Shared.Storages.Lib
{
    public interface IClientStorage
    {
        public Task SetClientSettingsAsync(ClientSettings value);
        public Task<ClientSettings?> GetClientSettingsAsync();
        public Task ClearClientSettingsAsync();
    }
}