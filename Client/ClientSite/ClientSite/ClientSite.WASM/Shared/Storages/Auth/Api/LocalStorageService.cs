using Microsoft.JSInterop;
using System.Text.Json;

namespace ClientSite.WASM.Shared.Storages.Auth.Api
{
    public class LocalStorageService : ILocalStorageService, IAsyncDisposable
    {
        #region Private Fields

        private readonly IJSRuntime _jsRuntime;
        private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

        #endregion

        #region Ctors

        public LocalStorageService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            _moduleTask = new Lazy<Task<IJSObjectReference>>(() =>
                jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/ClientSite.WASM/js/localStorage.js").AsTask());
        }

        #endregion

        #region Public methods

        public async Task<T?> GetItemAsync<T>(string key)
        {
            try
            {
                var module = await _moduleTask.Value;
                var json = await module.InvokeAsync<string>("getItem", key);

                if (string.IsNullOrEmpty(json))
                    return default;

                return JsonSerializer.Deserialize<T>(json);
            }
            catch
            {
                return default;
            }
        }

        public async Task SetItemAsync<T>(string key, T value)
        {
            var module = await _moduleTask.Value;
            var json = JsonSerializer.Serialize(value);
            await module.InvokeVoidAsync("setItem", key, json);
        }

        public async Task RemoveItemAsync(string key)
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("removeItem", key);
        }

        public async Task ClearAsync()
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("clear");
        }

        public async Task<bool> ContainsKeyAsync(string key)
        {
            try
            {
                var module = await _moduleTask.Value;
                return await module.InvokeAsync<bool>("containsKey", key);
            }
            catch
            {
                return false;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_moduleTask.IsValueCreated)
            {
                var module = await _moduleTask.Value;
                await module.DisposeAsync();
            }
        }

        #endregion
    }
}