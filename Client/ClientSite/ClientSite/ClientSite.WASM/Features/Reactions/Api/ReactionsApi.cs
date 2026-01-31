using Api.Interfaces;
using Api.Models.MServices.ContentMService.Endpoints.ReactionEndpoints.Requests;
using Api.Models.MServices.ContentMService.Endpoints.ReactionEndpoints.Responses;
using ClientSite.WASM.Shared.Services;

namespace ClientSite.WASM.Features.Reactions.Api
{
    public class ReactionsApi(IMicroservicesClient _client, IAuthenticatedApiService _authenticatedApiService)
    {
        public async Task<List<ReactionDTO>> GetReactionsByPost(Guid postId, CancellationToken cancellationToken = default)
            => (await _client.Content.Reaction.GetReactionsByPost(postId, cancellationToken)) ?? new List<ReactionDTO>();

        public Task<ReactionDTO> AddOrUpdateReaction(AddReactionRequest request, CancellationToken cancellationToken = default)
            => _authenticatedApiService.ExecuteWithTokenRefreshAsync(
                    token => _client.Content.Reaction.AddOrUpdateReaction(request, token, cancellationToken)
                );

        public Task RemoveReaction(Guid postId, CancellationToken cancellationToken = default)
            => _authenticatedApiService.ExecuteWithTokenRefreshAsync(
                    token => _client.Content.Reaction.RemoveReaction(postId, token, cancellationToken)
                );
    }
}