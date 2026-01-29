using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Comment.Requests
{
    public record CreateCommentRequest
    {
        [Required]
        public Guid PostId { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(2000)]
        public string Text { get; set; } = string.Empty;
    }
}