using BLL.DTOs.Comment.DTO;
using BLL.DTOs.Reaction.DTO;
using BLL.Integrations.IdentityMService.DTOs;

namespace BLL.DTOs.Post.DTO
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