using Api.Interfaces;
using Api.Models.MServices.ContentMService.Endpoints.CommentEndpoints.Requests;
using Api.Models.MServices.ContentMService.Endpoints.CommentEndpoints.Responses;
using ClientSite.WASM.Shared.Services;

namespace ClientSite.WASM.Features.Comments.Api
{
    public class CommentsApi(IMicroservicesClient _client, IAuthenticatedApiService _authenticatedApiService)
    {
        public Task<Guid> CreateComment(CreateCommentRequest request, CancellationToken cancellationToken = default)
            => _authenticatedApiService.ExecuteWithTokenRefreshAsync(
                    token => _client.Content.Comment.CreateComment(request, token, cancellationToken)
                );

        public Task<Guid> DeleteComment(Guid id, CancellationToken cancellationToken = default)
            => _authenticatedApiService.ExecuteWithTokenRefreshAsync(
                    token => _client.Content.Comment.DeleteComment(id, token, cancellationToken)
                );

        public async Task<List<CommentDTO>> GetCommentsByPost(Guid postId, CancellationToken cancellationToken = default)
            => (await _client.Content.Comment.GetCommentsByPost(postId, cancellationToken)) ?? new List<CommentDTO>();
    }
}