using ClientSite.SSR.Components;

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
