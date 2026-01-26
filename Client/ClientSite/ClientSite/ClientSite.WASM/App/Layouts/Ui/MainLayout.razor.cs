using System.Runtime.InteropServices;

namespace ClientSite.WASM.App.Layouts.Ui
{
    public partial class MainLayout
    {
        #region UI Fields

        private bool _isLoaded = false;
        private bool _isClientSide = false;

        #endregion

        #region LC Methods

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            CheckPlatform();
            await ConfirmClientLoadAsync();
        }

        #endregion

        #region Private methods

        private void CheckPlatform()
            => _isClientSide = RuntimeInformation.IsOSPlatform(OSPlatform.Create("Browser"));

        private async Task ConfirmClientLoadAsync()
        {
            if (_isClientSide)
                _isLoaded = true;
        }

        #endregion
    }
}