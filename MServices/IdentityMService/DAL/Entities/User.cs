using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    [Table("Users")]
    public record User
    {
        [Key]
        [Column("Id")]
        public Guid Id { get; init; }

        [Required]
        [Column("Email")]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; init; } = string.Empty;

        [Required]
        [Column("UserName")]
        [MaxLength(100)]
        public string Username { get; init; } = string.Empty;

        [Required]
        [Column("PasswordHash")]
        public string PasswordHash { get; set; } = string.Empty;

        [Column("IsActive")]
        public bool IsActive { get; init; } = true;

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; init; }

        [Column("UpdatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [Column("LastLoginAt")]
        public DateTime? LastLoginAt { get; set; }
    }
}