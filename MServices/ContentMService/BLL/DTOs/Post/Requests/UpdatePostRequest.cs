using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Post.Requests
{
    public record UpdatePostRequest
    {
        [Required]
        [MinLength(1)]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        public string Content { get; set; } = string.Empty;
    }
}