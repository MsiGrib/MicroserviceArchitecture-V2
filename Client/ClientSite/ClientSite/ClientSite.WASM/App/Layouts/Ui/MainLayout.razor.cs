using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ClientSite.WASM.App.Layouts.Ui
{
    public partial class MainLayout
    {
        #region Injects

        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        #endregion

        #region UI Fields

        private bool _isLoaded = false;
        private bool _isClient = false;

        #endregion

        #region LC Methods

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _isClient = OperatingSystem.IsBrowser();

            if (!_isClient)
            {
                _isLoaded = true;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender && _isClient)
            {
                try
                {
                    await JSRuntime.InvokeVoidAsync("eval", "console.log('Layout loaded on client')");

                    _isLoaded = true;
                    StateHasChanged();
                }
                catch
                {
                    _isLoaded = true;
                    StateHasChanged();
                }
            }
        }

        #endregion
    }
}