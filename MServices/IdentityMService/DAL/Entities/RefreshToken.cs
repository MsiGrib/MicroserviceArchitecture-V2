using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    [Table("RefreshTokens")]
    public class RefreshToken
    {
        [Key]
        [Column("Id")]
        public Guid Id { get; set; }

        [Required]
        [Column("Token")]
        public string Token { get; set; } = string.Empty;

        [Column("UserId")]
        public Guid UserId { get; set; }

        [Column("ExpiresAt")]
        public DateTime ExpiresAt { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("IsRevoked")]
        public bool IsRevoked { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
    }
}