using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    [Table("Posts")]
    public record Post
    {
        [Key]
        [Column("Id")]
        public Guid Id { get; init; }

        [Required]
        [Column("Title")]
        [MaxLength(255)]
        [EmailAddress]
        public string Title { get; init; } = string.Empty;

        [Required]
        [Column("Content")]
        [MaxLength(int.MaxValue)]
        [EmailAddress]
        public string Content { get; init; } = string.Empty;

        [Required]
        [Column("CreatedAt")]
        public DateTime CreatedAt { get; init; }

        [Column("UpdatedAt")]
        public DateTime? UpdatedAt { get; init; }

        [Column("DeletedAt")]
        public DateTime? DeletedAt { get; init; }

        [Required]
        [Column("UserId")]
        public Guid UserId { get; init; }

        public ICollection<Comment> Comments { get; init; } = new List<Comment>();
        public ICollection<Reaction> Reactions { get; init; } = new List<Reaction>();

        [NotMapped]
        public int ReactionCount => Reactions.Count;
        [NotMapped]
        public int CommentCount => Comments.Count;
    }
}