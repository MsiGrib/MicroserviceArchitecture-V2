namespace Api.Models
{
    public record ControllerConfig
    {
        public string BasePath { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public List<HttpMethod> SupportedMethods { get; init; } = new();
    }
}