namespace Api.Models.MServices.IdentityMService.Endpoints.UserEndpoints.Responses
{
    public record UserSmallInfoDTO
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }
}