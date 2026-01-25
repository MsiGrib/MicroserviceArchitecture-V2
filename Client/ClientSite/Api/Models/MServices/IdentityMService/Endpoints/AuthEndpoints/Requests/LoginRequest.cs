namespace Api.Models.MServices.IdentityMService.Endpoints.AuthEndpoints.Requests
{
    public record LoginRequest
    {
        public required string Login { get; init; }
        public required string Password { get; init; }
    }
}