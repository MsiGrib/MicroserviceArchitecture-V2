using Api;
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
            builder.Services.AddScoped<ClientStorage>();

            #endregion

            #endregion

            await builder.Build().RunAsync();
        }
    }
}