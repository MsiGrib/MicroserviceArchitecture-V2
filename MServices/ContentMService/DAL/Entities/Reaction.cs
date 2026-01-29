using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    [Table("Reactions")]
    public record Reaction
    {
        [Key]
        public Guid Id { get; init; }

        [Required]
        public int Type { get; init; } = 0;

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