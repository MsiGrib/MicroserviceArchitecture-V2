using ClientSite.WASM.Shared.Components.Texts.Lib;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace ClientSite.WASM.Shared.Components.Inputs
{
    public partial class BaseInput : BaseComponent
    {
        #region Params

        [Parameter] public string? Value { get; set; }
        [Parameter] public string? Placeholder { get; set; }
        [Parameter] public SimpleTextType SimpleTextType { get; set; } = SimpleTextType.H1;
        [Parameter] public bool ReadOnly { get; set; } = false;
        [Parameter] public EventCallback<string> ValueChanged { get; set; }
        [Parameter] public EventCallback<string> OnEnter { get; set; }
        [Parameter] public string Type { get; set; } = "text";

        #endregion

        #region UI Fields

        protected string _baseClass = string.Empty;

        #endregion

        #region LC Methods

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _baseClass = GetTextClass(SimpleTextType);
        }

        #endregion

        #region Private methods

        protected virtual async void OnInputChanged(ChangeEventArgs e)
        {
            if (ReadOnly) return;

            string input = e.Value?.ToString() ?? string.Empty;

            if (Value != input)
            {
                Value = input;
                if (ValueChanged.HasDelegate)
                    await ValueChanged.InvokeAsync(Value);
            }
        }

        protected virtual async Task OnKeyDown(KeyboardEventArgs e)
        {
            if (ReadOnly) return;

            if (e.Code == "Enter" || e.Key == "Enter")
            {
                if (OnEnter.HasDelegate)
                    await OnEnter.InvokeAsync(Value);
            }
        }

        #endregion
    }
}
