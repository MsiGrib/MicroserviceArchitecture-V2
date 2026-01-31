using Api.Interfaces;
using Api.Models.MServices.ContentMService.Endpoints.PostEndpoints.Requests;
using Api.Models.MServices.ContentMService.Endpoints.PostEndpoints.Responses;
using ClientSite.WASM.Shared.Services;

namespace ClientSite.WASM.Features.Posts.Api
{
    public class PostsApi(IMicroservicesClient _client, IAuthenticatedApiService _authenticatedApiService)
    {
        public async Task<List<PostDTO>> GetAllPosts(CancellationToken cancellationToken = default)
            => (await _client.Content.Post.GetAllPosts(cancellationToken)) ?? new List<PostDTO>();

        public Task<Guid> CreatePost(CreatePostRequest request, CancellationToken cancellationToken = default)
            => _authenticatedApiService.ExecuteWithTokenRefreshAsync(
                    token => _client.Content.Post.CreatePost(request, token, cancellationToken)
                );
    }
}