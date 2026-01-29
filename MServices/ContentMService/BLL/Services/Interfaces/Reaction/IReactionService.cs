using BLL.DTOs.Reaction.DTO;
using BLL.DTOs.Reaction.Requests;

namespace BLL.Services.Interfaces.Reaction
{
    public interface IReactionService
    {
        public Task<ReactionDTO> AddOrUpdateReactionAsync(AddReactionRequest request, Guid userId);
        public Task<bool> RemoveReactionAsync(Guid postId, Guid userId);
        public Task<IEnumerable<ReactionDTO>> GetReactionsByPostIdAsync(Guid postId);
    }
}