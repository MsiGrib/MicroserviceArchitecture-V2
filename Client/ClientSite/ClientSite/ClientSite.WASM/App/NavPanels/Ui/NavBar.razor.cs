using Microsoft.AspNetCore.Components;

namespace ClientSite.WASM.App.NavPanels.Ui
{
    public partial class NavBar
    {
        #region Injects

        [Inject] private NavigationManager Navigation { get; init; } = default!;

        #endregion

        #region UI Fields

        private bool _isAuthenticated = false;
        private string _userName = string.Empty;

        #endregion

        #region Private methods

        private void NavigateToHome()
        {
            Navigation.NavigateTo("/");
        }

        private void NavigateToLogin()
        {
            Navigation.NavigateTo("/login");
        }

        private async Task NavigateToRegister()
        {
            Navigation.NavigateTo("/SignUp");
        }

        private void LogOut()
        {

        }

        #endregion
    }
}