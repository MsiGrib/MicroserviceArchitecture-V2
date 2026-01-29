namespace Api.Models
{
    public record ServiceConfig
    {
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public Dictionary<string, ControllerConfig> Controllers { get; init; } = new();
    }
}