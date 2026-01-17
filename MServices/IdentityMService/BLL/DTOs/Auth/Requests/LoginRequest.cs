using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Auth.Requests
{
    public record LoginRequest
    {
        [Required]
        public string Login { get; init; } = string.Empty;

        [Required]
        public string Password { get; init; } = string.Empty;
    }
}