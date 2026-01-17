using ClientSite.WASM.Shared.Components.Texts.Lib;
using Microsoft.AspNetCore.Components;

namespace ClientSite.WASM.Shared.Components
{
    public abstract class BaseComponent : ComponentBase
    {
        #region Params

        [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }

        #endregion

        #region UI Fields

        protected string AdditionalClass { get; private set; } = string.Empty;
        protected Dictionary<string, object> CleanedAttributes { get; private set; } = new();

        #endregion

        #region LC Methods

        protected override void OnInitialized()
        {
            base.OnInitialized();

            var preparation = PreparationAdditionalAttributes(AdditionalAttributes);

            CleanedAttributes = preparation.Attributes;
            AdditionalClass = preparation.Class;
        }

        #endregion

        #region Private methods

        private (Dictionary<string, object> Attributes, string Class) PreparationAdditionalAttributes(Dictionary<string, object>? input)
        {
            var newAttributes = (input ?? new Dictionary<string, object>())
                .Where(kvp => kvp.Key != "class")
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var newClass = input != null && input.TryGetValue("class", out var cls)
                ? cls?.ToString() ?? string.Empty
                : string.Empty;

            return (newAttributes, newClass);
        }

        #endregion

        #region Protected methods

        protected string GetTextClass(SimpleTextType type)
            => type switch
            {
                SimpleTextType.H1 => @"
                    text-4xl md:text-5xl lg:text-6xl 
                    font-bold 
                    tracking-tight 
                    leading-tight",

                SimpleTextType.H2 => @"
                    text-3xl md:text-4xl lg:text-5xl 
                    font-bold 
                    tracking-tight 
                    leading-snug",

                SimpleTextType.H3 => @"
                    text-2xl md:text-3xl lg:text-4xl 
                    font-bold 
                    tracking-normal 
                    leading-snug",

                SimpleTextType.H4 => @"
                    text-xl md:text-2xl lg:text-3xl 
                    font-semibold 
                    tracking-normal 
                    leading-relaxed",

                SimpleTextType.H5 => @"
                    text-lg md:text-xl lg:text-2xl 
                    font-semibold 
                    tracking-normal 
                    leading-relaxed",

                SimpleTextType.H6 => @"
                    text-base md:text-lg lg:text-xl 
                    font-medium 
                    tracking-normal 
                    leading-relaxed",

                SimpleTextType.Minimal1 => @"
                    text-xs md:text-sm 
                    font-medium 
                    tracking-normal 
                    leading-tight",

                SimpleTextType.Button1 => @"
                    text-base md:text-lg 
                    font-bold 
                    tracking-wide 
                    leading-none 
                    uppercase",

                SimpleTextType.Button2 => @"
                    text-sm md:text-base 
                    font-semibold 
                    tracking-normal 
                    leading-tight",

                _ => @"
                    text-base 
                    font-normal 
                    tracking-normal 
                    leading-normal"
            };

        #endregion
    }
}