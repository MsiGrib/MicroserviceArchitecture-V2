using Api;
using ClientSite.SSR.Components;
using ClientSite.WASM.Shared.Services;
using ClientSite.WASM.Shared.Storages.Lib;

namespace ClientSite.SSR
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Builder

            #region Main

            builder.Services
                .AddRazorComponents()
                .AddInteractiveWebAssemblyComponents();

            builder.Services.AddMicroservicesIntegrationApi(builder.Configuration);

            #endregion

            #region Additionally

            builder.Services.AddScoped<StorageService>(sp => new StorageService(null));
            builder.Services.AddScoped<IClientStorage, ServerClientStorage>();
            builder.Services.AddScoped<IAuthTokenService, AuthTokenService>();
            builder.Services.AddScoped<IAuthStateService, AuthStateService>();
            builder.Services.AddScoped<IAuthenticatedApiService, AuthenticatedApiService>();

            #endregion

            #endregion

            var app = builder.Build();

            #region App

            #region Main

            if (app.Environment.IsDevelopment())
                app.UseWebAssemblyDebugging();
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(WASM._Imports).Assembly);

            #endregion

            #endregion

            app.Run();
        }
    }
}
