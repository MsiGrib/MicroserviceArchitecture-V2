namespace BLL.DTOs.User.DTO
{
    public record UserDTO
    {
        public Guid Id { get; init; }
        public string Email { get; init; } = string.Empty;
        public string Username { get; init; } = string.Empty;
        public bool IsActive { get; init; } = true;
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public DateTime? LastLoginAt { get; init; }
    }
}