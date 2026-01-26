using ClientSite.WASM.Shared.Storages.Model;

namespace ClientSite.WASM.Shared.Storages.Lib
{
    public class ServerClientStorage : IClientStorage
    {
        #region Public methods

        public Task SetClientSettingsAsync(ClientSettings value)
            => Task.CompletedTask;

        public Task<ClientSettings?> GetClientSettingsAsync()
            => Task.FromResult<ClientSettings?>(null);

        public Task ClearClientSettingsAsync()
            => Task.CompletedTask;

        #endregion
    }
}