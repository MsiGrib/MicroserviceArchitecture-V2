namespace BLL.Integrations.IdentityMService.DTOs
{
    public record UserSmallInfoDTO
    {
        public Guid Id { get; init; }
        public string Email { get; init; } = string.Empty;
        public string Username { get; init; } = string.Empty;
    }
}