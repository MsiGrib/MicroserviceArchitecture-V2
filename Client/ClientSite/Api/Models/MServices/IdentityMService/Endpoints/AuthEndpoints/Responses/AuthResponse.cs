namespace Api.Models.MServices.IdentityMService.Endpoints.AuthEndpoints.Responses
{
    public record AuthResponse
    {
        public string AccessToken { get; init; } = string.Empty;
        public string RefreshToken { get; init; } = string.Empty;
        public int ExpiresIn { get; init; }
        public Guid UserId { get; init; }
        public string Email { get; init; } = string.Empty;
        public string Username { get; init; } = string.Empty;
    }
}