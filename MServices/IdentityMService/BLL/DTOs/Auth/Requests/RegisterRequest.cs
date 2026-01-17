using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Auth.Requests
{
    public record RegisterRequest
    {
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; init; } = string.Empty;

        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string Username { get; init; } = string.Empty;

        [Required]
        [MinLength(8)]
        [MaxLength(100)]
        public string Password { get; init; } = string.Empty;
    }
}