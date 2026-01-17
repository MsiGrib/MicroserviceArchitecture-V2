namespace ClientSite.WASM.Shared.Storages.Auth.Api
{
    public interface ILocalStorageService
    {
        public Task<T?> GetItemAsync<T>(string key);
        public Task SetItemAsync<T>(string key, T value);
        public Task RemoveItemAsync(string key);
        public Task ClearAsync();
        public Task<bool> ContainsKeyAsync(string key);
    }
}