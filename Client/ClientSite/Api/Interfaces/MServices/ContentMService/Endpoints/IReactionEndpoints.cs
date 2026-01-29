using Api.Models.MServices.ContentMService.Endpoints.ReactionEndpoints.Requests;
using Api.Models.MServices.ContentMService.Endpoints.ReactionEndpoints.Responses;

namespace Api.Interfaces.MServices.ContentMService.Endpoints
{
    public interface IReactionEndpoints
    {
        public Task<ReactionDTO> AddOrUpdateReaction(AddReactionRequest request, string token, CancellationToken cancellationToken = default);
        public Task RemoveReaction(Guid postId, string token, CancellationToken cancellationToken = default);
        public Task<List<ReactionDTO>> GetReactionsByPost(Guid postId, CancellationToken cancellationToken = default);
    }
}