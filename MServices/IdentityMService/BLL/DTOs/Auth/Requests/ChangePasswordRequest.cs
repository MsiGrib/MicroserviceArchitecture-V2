using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Auth.Requests
{
    public class ChangePasswordRequest
    {
        [Required]
        public string CurrentPassword { get; init; } = string.Empty;

        [Required]
        public string NewPassword { get; init; } = string.Empty;

        [Required]
        public string ConfirmNewPassword { get; init; } = string.Empty;
    }
}