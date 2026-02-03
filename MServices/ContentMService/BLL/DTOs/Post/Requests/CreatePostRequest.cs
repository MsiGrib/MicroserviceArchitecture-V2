using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Post.Requests
{
    public record CreatePostRequest
    {
        [Required]
        [MinLength(1)]
        [MaxLength(255)]
        public string Title { get; init; } = string.Empty;

        [Required]
        [MinLength(1)]
        public string Content { get; init; } = string.Empty;
    }
}