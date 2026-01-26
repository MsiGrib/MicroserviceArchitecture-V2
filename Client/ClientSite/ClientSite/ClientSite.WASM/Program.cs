using Api;
using ClientSite.WASM.Shared.Services;
using ClientSite.WASM.Shared.Storages.Lib;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace ClientSite.WASM
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            #region Builder

            #region Main

            builder.Services.AddMicroservicesIntegrationApi(builder.Configuration);

            #endregion

            #region Additionally

            builder.Services.AddScoped<StorageService>();
            builder.Services.AddScoped<IClientStorage, ClientStorage>();
            builder.Services.AddScoped<IAuthTokenService, AuthTokenService>();
            builder.Services.AddScoped<IAuthStateService, AuthStateService>();
            builder.Services.AddScoped<IAuthenticatedApiService, AuthenticatedApiService>();

            #endregion

            #endregion

            await builder.Build().RunAsync();
        }
    }
}