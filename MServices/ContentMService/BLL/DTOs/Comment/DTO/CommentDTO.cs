using BLL.Integrations.IdentityMService.DTOs;

namespace BLL.DTOs.Comment.DTO
{
    public record CommentDTO
    {
        public Guid Id { get; init; }
        public string Text { get; init; } = string.Empty;
        public UserSmallInfoDTO UserInfo { get; init; } = new();
        public Guid PostId { get; init; }
    }
}