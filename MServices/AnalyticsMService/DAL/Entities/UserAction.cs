using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    public abstract class UserAction
    {
        [Key]
        [Column("Id")]
        public Guid Id { get; init; }

        [Required]
        [Column("UserId")]
        public Guid UserId { get; init; }

        [Required]
        [Column("UTC")]
        public DateTime UTC { get; init; }

        [Required]
        [Column("Local")]
        public DateTime? Local { get; init; } = null;

        [MaxLength(100)]
        [Column("TimeZone")]
        public string? TimeZone { get; init; } = null;

        [MaxLength(45)]
        [Column("IpAddress")]
        public string? IpAddress { get; init; } = null;

        [MaxLength(1000)]
        [Column("UserAgent")]
        public string? UserAgent { get; init; } = null;
    }
}