using Api.Models.MServices.ContentMService.Endpoints.CommentEndpoints.Responses;
using Api.Models.MServices.ContentMService.Endpoints.ReactionEndpoints.Responses;
using Api.Models.MServices.IdentityMService.Endpoints.UserEndpoints.Responses;

namespace Api.Models.MServices.ContentMService.Endpoints.PostEndpoints.Responses
{
    public record PostDTO
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Content { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        public UserSmallInfoDTO UserInfo { get; init; } = new();

        public ICollection<CommentDTO> Comments { get; init; } = new List<CommentDTO>();
        public ICollection<ReactionDTO> Reactions { get; init; } = new List<ReactionDTO>();
    }
}