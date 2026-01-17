namespace Common.Models
{
    public record JwtSettings
    {
        public string Key { get; init; } = string.Empty;
        public string Issuer { get; init; } = string.Empty;
        public string Audience { get; init; } = string.Empty;
        public int AccessTokenExpirationMinutes { get; init; } = 60;
        public int RefreshTokenExpirationDays { get; init; } = 7;
    }
}