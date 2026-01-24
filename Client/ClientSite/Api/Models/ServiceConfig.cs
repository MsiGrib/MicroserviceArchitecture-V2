namespace Api.Models
{
    public record ServiceConfig
    {
        public string AuthBasePath { get; init; } = "/api/auth";
        public string UserBasePath { get; init; } = "/api/user";
    }
}