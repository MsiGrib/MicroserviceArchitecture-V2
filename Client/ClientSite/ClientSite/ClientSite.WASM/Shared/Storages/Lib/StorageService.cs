using ClientSite.WASM.Shared.Storages.Model;
using Microsoft.JSInterop;
using System.Text.Json;

namespace ClientSite.WASM.Shared.Storages.Lib
{
    public class StorageService
    {
        private readonly IJSRuntime? _jsRuntime = null;
        private bool _isBrowser = false;

        public StorageService(IJSRuntime? jsRuntime = null)
        {
            _jsRuntime = jsRuntime;
            _isBrowser = jsRuntime != null;
        }

        public async Task Set<T>(string key, T value)
        {
            if (!_isBrowser || _jsRuntime == null) return;

            var item = new StoredValue<T>
            {
                Value = value,
                StoredAt = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(item);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, json);
        }

        public async Task<T?> Get<T>(string key, int minutesToLive)
        {
            if (!_isBrowser || _jsRuntime == null) return default;

            try
            {
                var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
                if (string.IsNullOrWhiteSpace(json))
                    return default;

                var item = JsonSerializer.Deserialize<StoredValue<T>>(json);
                if (item == null)
                    return default;

                var age = DateTime.UtcNow - item.StoredAt;
                if (age.TotalMinutes > minutesToLive)
                {
                    await Remove(key);
                    return default;
                }

                return item.Value;
            }
            catch
            {
                return default;
            }
        }

        public async Task Remove(string key)
        {
            if (!_isBrowser || _jsRuntime == null) return;
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }

        public async Task Clear()
        {
            if (!_isBrowser || _jsRuntime == null) return;
            await _jsRuntime.InvokeVoidAsync("localStorage.clear");
        }
    }
}