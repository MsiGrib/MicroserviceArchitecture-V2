using ClientSite.WASM.Shared.Components.Texts.Lib;
using Microsoft.AspNetCore.Components;

namespace ClientSite.WASM.Shared.Components.Texts.Ui
{
    public partial class SimpleText : BaseComponent
    {
        #region Params

        [Parameter] public SimpleTextType Type { get; set; } = SimpleTextType.H1;
        [Parameter] public RenderFragment? ChildContent { get; set; }

        #endregion

        #region UI Fields

        private string _baseClass = string.Empty;

        #endregion

        #region LC Methods

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _baseClass = GetTextClass(Type);
        }

        #endregion
    }
}