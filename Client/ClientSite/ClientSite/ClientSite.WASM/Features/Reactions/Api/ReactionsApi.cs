using Api.Interfaces;
using Api.Models.MServices.ContentMService.Endpoints.ReactionEndpoints.Requests;
using Api.Models.MServices.ContentMService.Endpoints.ReactionEndpoints.Responses;
using ClientSite.WASM.Shared.Services;

namespace ClientSite.WASM.Features.Reactions.Api
{
    public class ReactionsApi(IMicroservicesClient client, IAuthenticatedApiService authenticatedApiService)
    {
        private readonly IMicroservicesClient _client = client;
        private readonly IAuthenticatedApiService _authenticatedApiService = authenticatedApiService;

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