using System.Runtime.InteropServices;

namespace ClientSite.WASM.App.Layouts.Ui
{
    public partial class MainLayout
    {
        private bool _isLoaded = false;
        private bool _isClientSide = false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            CheckPlatform();
            await ConfirmClientLoadAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
        }

        private void CheckPlatform()
            => _isClientSide = RuntimeInformation.IsOSPlatform(OSPlatform.Create("Browser"));

        private async Task ConfirmClientLoadAsync()
        {
            if (_isClientSide)
            {
                _isLoaded = true;
            }
        }
    }
}