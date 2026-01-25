namespace Api.Models.MServices.IdentityMService.Endpoints.AuthEndpoints.Requests
{
    public record RefreshTokenRequest
    {
        public required string RefreshToken { get; init; }
    }
}