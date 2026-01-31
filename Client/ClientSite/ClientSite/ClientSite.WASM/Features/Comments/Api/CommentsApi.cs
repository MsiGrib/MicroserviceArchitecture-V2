using Api.Interfaces;
using Api.Models.MServices.ContentMService.Endpoints.CommentEndpoints.Requests;
using Api.Models.MServices.ContentMService.Endpoints.CommentEndpoints.Responses;
using ClientSite.WASM.Shared.Services;

namespace ClientSite.WASM.Features.Comments.Api
{
    public class CommentsApi(IMicroservicesClient client, IAuthenticatedApiService authenticatedApiService)
    {
        private readonly IMicroservicesClient _client = client;
        private readonly IAuthenticatedApiService _authenticatedApiService = authenticatedApiService;

        public Task<Guid> CreateComment(CreateCommentRequest request, CancellationToken cancellationToken = default)
            => _authenticatedApiService.ExecuteWithTokenRefreshAsync(
                    token => _client.Content.Comment.CreateComment(request, token, cancellationToken)
                );

        public Task<Guid> DeleteComment(Guid id, CancellationToken cancellationToken = default)
            => _authenticatedApiService.ExecuteWithTokenRefreshAsync(
                    token => _client.Content.Comment.DeleteComment(id, token, cancellationToken)
                );

        public Task<List<CommentDTO>> GetCommentsByPost(Guid postId, CancellationToken cancellationToken = default)
            => _client.Content.Comment.GetCommentsByPost(postId, cancellationToken);
    }
}