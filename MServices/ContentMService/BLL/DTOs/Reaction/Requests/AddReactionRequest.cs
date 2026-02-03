using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Reaction.Requests
{
    public record AddReactionRequest
    {
        [Required]
        public Guid PostId { get; init; }

        [Required]
        [Range(0, int.MaxValue)]
        public int ReactionType { get; init; }
    }
}