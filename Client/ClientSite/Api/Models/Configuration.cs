namespace Api.Models
{
    public record Configuration
    {
        public const string ConfigurationSection = "MicroservicesApi";
        public string ApiGatewayBaseUrl { get; init; } = string.Empty;
        public Dictionary<string, ServiceConfig> Services { get; init; } = new();
    }
}