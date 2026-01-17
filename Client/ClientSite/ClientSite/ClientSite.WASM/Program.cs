using ClientSite.WASM.Shared.Storages.Auth.Api;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace ClientSite.WASM
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            #region Builder

            #region Additionally

            builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
            builder.Services.AddScoped<IAuthStorageService, AuthStorageService>();

            #endregion

            #endregion

            await builder.Build().RunAsync();
        }
    }
}