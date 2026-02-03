using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Comment.Requests
{
    public record CreateCommentRequest
    {
        [Required]
        public Guid PostId { get; init; }

        [Required]
        [MinLength(1)]
        [MaxLength(2000)]
        public string Text { get; init; } = string.Empty;
    }
}