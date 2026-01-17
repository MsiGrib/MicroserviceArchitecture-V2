using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Auth.Requests
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; init; } = string.Empty;
    }
}