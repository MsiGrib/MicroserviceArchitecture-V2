namespace Common.Models
{
    public record AppSettings
    {
        public JwtSettings Jwt { get; init; } = new();
        public RedisSettings Redis { get; init; } = new();
        public ConnectionStrings ConnectionStrings { get; init; } = new();
    }
}