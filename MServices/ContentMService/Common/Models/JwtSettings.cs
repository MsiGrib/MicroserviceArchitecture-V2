namespace Common.Models
{
    public record JwtSettings
    {
        public string Key { get; init; } = string.Empty;
    }
}