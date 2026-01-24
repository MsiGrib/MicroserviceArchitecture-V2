namespace Api.Models.MServices.IdentityMService.Endpoints.AuthEndpoints.Requests
{
    public record RegisterRequest
    {
        public required string Email { get; init; } = string.Empty;
        public required string Username { get; init; } = string.Empty;
        public required string Password { get; init; } = string.Empty;
    }
}