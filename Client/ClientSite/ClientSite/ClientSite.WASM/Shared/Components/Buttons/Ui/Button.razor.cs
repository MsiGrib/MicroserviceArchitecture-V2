using ClientSite.WASM.Shared.Components.Texts.Lib;
using Microsoft.AspNetCore.Components;

namespace ClientSite.WASM.Shared.Components.Buttons.Ui
{
    public partial class Button : BaseComponent
    {
        #region Params

        [Parameter] public SimpleTextType SimpleTextType { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public EventCallback OnClick { get; set; }

        #endregion
    }
}