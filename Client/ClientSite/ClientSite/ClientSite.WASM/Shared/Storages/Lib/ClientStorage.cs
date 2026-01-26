using ClientSite.WASM.Shared.Storages.Model;

namespace ClientSite.WASM.Shared.Storages.Lib
{
    public class ClientStorage(StorageService _storage) : IClientStorage
    {
        #region Private Fields

        private const string ClientSettingsKey = "clientSettings";
        private const int ClientSettingsMinutesToLive = 9999;

        #endregion

        #region IClientStorage implementation

        public async Task SetClientSettingsAsync(ClientSettings value)
            => await _storage.Set(ClientSettingsKey, value);

        public async Task<ClientSettings?> GetClientSettingsAsync()
            => await _storage.Get<ClientSettings>(ClientSettingsKey, ClientSettingsMinutesToLive);

        public async Task ClearClientSettingsAsync()
            => await _storage.Remove(ClientSettingsKey);

        #endregion
    }
}