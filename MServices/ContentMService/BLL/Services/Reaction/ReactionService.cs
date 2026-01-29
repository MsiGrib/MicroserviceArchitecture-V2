using BLL.DTOs.Reaction.DTO;
using BLL.DTOs.Reaction.Requests;
using BLL.Services.Interfaces.Reaction;
using DAL.Repositories.Interfaces.Post;
using DAL.Repositories.Interfaces.Reaction;

namespace BLL.Services.Reaction
{
    public class ReactionService : IReactionService
    {
        private readonly IReactionRepository _reactionRepository;
        private readonly IPostRepository _postRepository;

        public ReactionService(IReactionRepository reactionRepository, IPostRepository postRepository)
        {
            _reactionRepository = reactionRepository;
            _postRepository = postRepository;
        }

        public async Task<ReactionDTO> AddOrUpdateReactionAsync(AddReactionRequest request, Guid userId)
        {
            var postExists = await _postRepository.ExistsAsync(request.PostId);
            if (!postExists)
                throw new InvalidOperationException("Post not found");

            var existingReaction = await _reactionRepository.GetByUserAndPostAsync(userId, request.PostId);

            if (existingReaction != null)
            {
                var updatedReaction = existingReaction with
                {
                    Type = request.ReactionType,
                };

                _reactionRepository.Update(updatedReaction);
                await _reactionRepository.SaveChangesAsync();

                return new ReactionDTO
                {
                    Id = updatedReaction.Id,
                    Type = updatedReaction.Type,
                    UserId = updatedReaction.UserId,
                    PostId = updatedReaction.PostId,
                };
            }
            else
            {
                var reaction = new DAL.Entities.Reaction
                {
                    Id = Guid.NewGuid(),
                    PostId = request.PostId,
                    UserId = userId,
                    Type = request.ReactionType,
                };

                await _reactionRepository.AddAsync(reaction);
                await _reactionRepository.SaveChangesAsync();

                return new ReactionDTO
                {
                    Id = reaction.Id,
                    Type = reaction.Type,
                    UserId = reaction.UserId,
                    PostId = reaction.PostId,
                };
            }
        }

        public async Task<bool> RemoveReactionAsync(Guid postId, Guid userId)
            => await _reactionRepository.RemoveByUserAndPostAsync(userId, postId)
                && await _reactionRepository.SaveChangesAsync();

        public async Task<IEnumerable<ReactionDTO>> GetReactionsByPostIdAsync(Guid postId)
        {
            var reactions = await _reactionRepository.GetByPostIdAsync(postId);

            return reactions.Select(x => new ReactionDTO
            {
                Id = x.Id,
                Type = x.Type,
                UserId = x.UserId,
                PostId = x.PostId,
            });
        }
    }
}