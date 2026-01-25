using Api.Interfaces;
using ClientSite.WASM.App.NavPanels.Api;
using ClientSite.WASM.Shared.Storages.Lib;
using Microsoft.AspNetCore.Components;

namespace ClientSite.WASM.App.NavPanels.Ui
{
    public partial class NavBar
    {
        #region Injects

        [Inject] private IMicroservicesClient MicroservicesClient { get; init; } = default!;
        [Inject] private NavigationManager Navigation { get; init; } = default!;
        [Inject] private ClientStorage ClientStorage { get; init; } = default!;

        #endregion

        #region UI Fields

        private NavBarApi? _api = null;
        private bool _isAuthenticated = false;

        #endregion

        #region LC Methods

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _api = new NavBarApi(MicroservicesClient);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            var clientSettings = await ClientStorage.GetClientSettingsAsync();

            _isAuthenticated = clientSettings != null &&
                !string.IsNullOrWhiteSpace(clientSettings.AccessToken) && !string.IsNullOrWhiteSpace(clientSettings.RefreshToken);
            StateHasChanged();
        }

        #endregion

        #region Private methods

        private void NavigateToHome()
        {
            Navigation.NavigateTo("/");
        }

        private void NavigateToLogin()
        {
            Navigation.NavigateTo("/SignIn");
        }

        private async Task NavigateToRegister()
        {
            Navigation.NavigateTo("/SignUp");
        }

        private async Task LogOut()
        {
            try
            {
                var clientSettings = await ClientStorage.GetClientSettingsAsync();
                if (clientSettings != null && !string.IsNullOrWhiteSpace(clientSettings.RefreshToken))
                    await _api!.LogOut(clientSettings.RefreshToken);

                await ClientStorage.ClearClientSettingsAsync();
                Navigation.NavigateTo("/");
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #endregion
    }
}