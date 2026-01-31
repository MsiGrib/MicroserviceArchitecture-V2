using Api.Models.MServices.IdentityMService.Endpoints.UserEndpoints.Responses;

namespace Api.Models.MServices.ContentMService.Endpoints.CommentEndpoints.Responses
{
    public record CommentDTO
    {
        public Guid Id { get; init; }
        public string Text { get; init; } = string.Empty;
        public UserSmallInfoDTO UserInfo { get; init; } = new();
        public Guid PostId { get; init; }
    }
}