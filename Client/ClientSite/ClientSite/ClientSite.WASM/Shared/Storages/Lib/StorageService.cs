using ClientSite.WASM.Shared.Storages.Model;
using Microsoft.JSInterop;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace ClientSite.WASM.Shared.Storages.Lib
{
    public class StorageService(IJSRuntime _js)
    {
        #region Public methods

        public async Task Set<T>(string key, T value)
        {
            var item = new StoredValue<T>
            {
                Value = value,
                StoredAt = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(item);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("Browser")))
                await _js.InvokeVoidAsync("localStorage.setItem", key, json);
        }

        public async Task<T?> Get<T>(string key, int minutesToLive)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Create("Browser")))
                return default;

            var json = await _js.InvokeAsync<string>("localStorage.getItem", key);
            if (string.IsNullOrWhiteSpace(json))
                return default;

            try
            {
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
                await Remove(key);
                return default;
            }
        }

        public async Task Remove(string key)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("Browser")))
                await _js.InvokeVoidAsync("localStorage.removeItem", key);
        }

        public async Task Clear()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("Browser")))
                await _js.InvokeVoidAsync("localStorage.clear");
        }

        #endregion
    }
}