using Api.Interfaces;
using ClientSite.WASM.App.NavPanels.Api;
using ClientSite.WASM.Shared.Services;
using ClientSite.WASM.Shared.Storages.Lib;
using Microsoft.AspNetCore.Components;

namespace ClientSite.WASM.App.NavPanels.Ui
{
    public partial class NavBar : IDisposable
    {
        #region Injects

        [Inject] private IMicroservicesClient MicroservicesClient { get; init; } = default!;
        [Inject] private IClientStorage ClientStorage { get; init; } = default!;
        [Inject] private IAuthStateService AuthStateService { get; init; } = default!;
        [Inject] private IAuthenticatedApiService AuthenticatedApiService { get; init; } = default!;
        [Inject] private NavigationManager Navigation { get; init; } = default!;

        #endregion

        #region UI Fields

        private NavBarApi? _api = null;
        private bool _isAuthenticated = false;
        private bool _isInitialized = false;
        private bool _disposed = false;
        private CancellationTokenSource? _cts;

        #endregion

        #region LC Methods

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _api = new NavBarApi(MicroservicesClient);
            _cts = new CancellationTokenSource();

            AuthStateService.OnAuthStateChanged += HandleAuthStateChanged;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender && !_disposed)
            {
                await CheckAuthState();
                _isInitialized = true;

                StateHasChanged();
            }
        }

        #endregion

        #region Private methods

        private async void HandleAuthStateChanged()
        {
            if (_disposed || _cts?.IsCancellationRequested == true) return;

            await InvokeAsync(async () =>
            {
                await CheckAuthState();
                StateHasChanged();
            });
        }

        private async Task CheckAuthState()
        {
            try
            {
                _isAuthenticated = await AuthStateService.CheckAuthenticationAsync();
            }
            catch
            {
                _isAuthenticated = false;
            }
        }


        private void NavigateToHome()
            => Navigation.NavigateTo("/");

        private void NavigateToLogin()
            => Navigation.NavigateTo("/SignIn");

        private async Task NavigateToRegister()
            => Navigation.NavigateTo("/SignUp");

        private async Task LogOut()
        {
            try
            {
                var clientSettings = await ClientStorage.GetClientSettingsAsync();
                if (clientSettings != null && !string.IsNullOrWhiteSpace(clientSettings.RefreshToken))
                {
                    await AuthenticatedApiService.ExecuteWithTokenRefreshAsync(async token =>
                    {
                        await _api!.LogOut(clientSettings.RefreshToken, token);
                    });
                }
            }
            catch (UnauthorizedAccessException) { }
            catch { }

            await AuthStateService.LogoutAsync();
            Navigation.NavigateTo("/");
        }

        #endregion

        #region Public methods

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _cts?.Cancel();
                _cts?.Dispose();

                AuthStateService.OnAuthStateChanged -= HandleAuthStateChanged;
            }
        }

        #endregion
    }
}