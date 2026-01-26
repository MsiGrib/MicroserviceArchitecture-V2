namespace ClientSite.WASM.Shared.Services
{
    public interface IAuthenticatedApiService
    {
        public Task<T> ExecuteWithTokenRefreshAsync<T>(Func<string, Task<T>> apiCall);
        public Task ExecuteWithTokenRefreshAsync(Func<string, Task> apiCall);
    }
}