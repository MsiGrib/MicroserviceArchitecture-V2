using Api.Interfaces;
using ClientSite.WASM.Shared.Storages.Lib;
using ClientSite.WASM.Shared.Storages.Model;
using ClientSite.WASM.Widgets.Auth.Api;
using Microsoft.AspNetCore.Components;

namespace ClientSite.WASM.Widgets.Auth.Ui
{
    public partial class SignInPanel
    {
        #region Injects

        [Inject] private IMicroservicesClient MicroservicesClient { get; init; } = default!;
        [Inject] private ClientStorage ClientStorage { get; init; } = default!;

        #endregion

        #region UI Fields

        private SignInPanelApi? _api = null;
        private string _userNameOrEmail = string.Empty;
        private string _password = string.Empty;

        #endregion

        #region LC Methods

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _api = new SignInPanelApi(MicroservicesClient);
        }

        #endregion

        #region Private methods

        private void UsernameOrEmailHandler(string userNameOrEmail)
            => _userNameOrEmail = userNameOrEmail;

        private void PasswordHandler(string password)
            => _password = password;

        private async Task OnSignUpClick()
        {
            try
            {
                var response = await _api!.Login(_userNameOrEmail, _password);

                if (response != null)
                {
                    await ClientStorage.ClearClientSettingsAsync();
                    await ClientStorage.SetClientSettingsAsync(new ClientSettings
                    {
                        AccessToken = response.AccessToken,
                        RefreshToken = response.RefreshToken,
                        ExpiresIn = response.ExpiresIn,
                        UserId = response.UserId,
                        Email = response.Email,
                        Username = response.Username,
                    });
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #endregion
    }
}