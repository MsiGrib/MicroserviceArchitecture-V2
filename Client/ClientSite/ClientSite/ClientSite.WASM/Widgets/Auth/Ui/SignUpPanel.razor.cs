using Api.Interfaces;
using ClientSite.WASM.Shared.Storages.Lib;
using ClientSite.WASM.Shared.Storages.Model;
using ClientSite.WASM.Widgets.Auth.Api;
using Microsoft.AspNetCore.Components;

namespace ClientSite.WASM.Widgets.Auth.Ui
{
    public partial class SignUpPanel
    {
        #region Injects

        [Inject] private IMicroservicesClient MicroservicesClient { get; init; } = default!;
        [Inject] private ClientStorage ClientStorage { get; init; } = default!;

        #endregion

        #region UI Fields

        private SignUpPanelApi? _api = null;
        private string _userName = string.Empty;
        private string _email = string.Empty;
        private string _password = string.Empty;

        #endregion

        #region LC Methods

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _api = new SignUpPanelApi(MicroservicesClient);
        }

        #endregion

        #region Private methods

        private void UsernameHandler(string userName)
            => _userName = userName;

        private void EmailHandler(string email)
            => _email = email;

        private void PasswordHandler(string password)
            => _password = password;

        private async Task OnSignUpClick()
        {
            try
            {
                var response = await _api!.SignUpAsync(_userName, _email, _password);

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