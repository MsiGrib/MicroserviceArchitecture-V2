using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    [Table("Comments")]
    public record Comment
    {
        [Key]
        [Column("Id")]
        public Guid Id { get; init; }

        [Required]
        [MaxLength(2000)]
        public string Text { get; init; } = string.Empty;

        [Required]
        [Column("UserId")]
        public Guid UserId { get; init; }

        [Required]
        [Column("PostId")]
        public Guid PostId { get; init; }

        [ForeignKey("PostId")]
        public Post? Post { get; init; }
    }
}