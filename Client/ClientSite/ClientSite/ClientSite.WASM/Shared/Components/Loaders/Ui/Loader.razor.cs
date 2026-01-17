using ClientSite.WASM.Shared.Components.Loaders.Lib;
using ClientSite.WASM.Shared.Components.Texts.Lib;
using Microsoft.AspNetCore.Components;

namespace ClientSite.WASM.Shared.Components.Loaders.Ui
{
    public partial class Loader : BaseComponent
    {
        #region Params

        [Parameter] public LoaderType Type { get; set; } = LoaderType.Spinner;
        [Parameter] public LoaderSize Size { get; set; } = LoaderSize.Medium;
        [Parameter] public string Color { get; set; } = "text-blue-600";
        [Parameter] public string BackgroundColor { get; set; } = "text-gray-200";
        [Parameter] public string Text { get; set; } = string.Empty;
        [Parameter] public SimpleTextType TextType { get; set; } = SimpleTextType.Minimal1;
        [Parameter] public bool ShowText { get; set; } = true;
        [Parameter] public bool Centered { get; set; } = true;
        [Parameter] public bool FullScreen { get; set; } = false;

        #endregion

        #region Protected methods

        protected string GetSizeClass()
            => Size switch
            {
                LoaderSize.Small => "w-6 h-6 border-3",
                LoaderSize.Medium => "w-10 h-10 border-4",
                LoaderSize.Large => "w-16 h-16 border-6",
                LoaderSize.ExtraLarge => "w-24 h-24 border-8",
                _ => "w-10 h-10 border-4"
            };

        protected string GetLoaderClasses()
        {
            var baseClasses = Type switch
            {
                LoaderType.Spinner => $"rounded-full border-solid border-t-transparent animate-spin {BackgroundColor}",
                LoaderType.Dots => $"flex space-x-2",
                LoaderType.Pulse => $"rounded-full animate-pulse",
                LoaderType.Bar => $"w-full h-1.5 rounded-full overflow-hidden",
                LoaderType.Circle => $"rounded-full border-dashed border-2 animate-spin",
                LoaderType.DualRing => $"rounded-full border-double border-4 animate-spin",
                LoaderType.Heart => $"animate-bounce",
                _ => $"rounded-full border-solid border-t-transparent animate-spin {BackgroundColor}"
            };

            return $"{GetSizeClass()} {baseClasses} {Color}";
        }

        protected string GetContainerClasses()
        {
            var classes = "flex flex-col items-center justify-center";

            if (Centered)
                classes += " absolute inset-0";

            if (FullScreen)
                classes += " fixed inset-0 bg-white/80 dark:bg-gray-900/80 backdrop-blur-sm z-50";

            return classes;
        }

        #endregion
    }
}