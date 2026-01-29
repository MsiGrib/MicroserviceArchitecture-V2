namespace BLL.DTOs.Comment.DTO
{
    public record CommentDTO
    {
        public Guid Id { get; init; }
        public string Text { get; init; } = string.Empty;
        public Guid UserId { get; init; }
        public Guid PostId { get; init; }
    }
}