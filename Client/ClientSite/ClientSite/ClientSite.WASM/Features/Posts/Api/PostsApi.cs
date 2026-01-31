using Api.Interfaces;
using Api.Models.MServices.ContentMService.Endpoints.PostEndpoints.Requests;
using Api.Models.MServices.ContentMService.Endpoints.PostEndpoints.Responses;
using ClientSite.WASM.Shared.Services;

namespace ClientSite.WASM.Features.Posts.Api
{
    public class PostsApi(IMicroservicesClient client, IAuthenticatedApiService authenticatedApiService)
    {
        private readonly IMicroservicesClient _client = client;
        private readonly IAuthenticatedApiService _authenticatedApiService = authenticatedApiService;

        public Task<List<PostDTO>> GetAllPosts(CancellationToken cancellationToken = default)
            => _client.Content.Post.GetAllPosts(cancellationToken);

        public Task<Guid> CreatePost(CreatePostRequest request, CancellationToken cancellationToken = default)
            => _authenticatedApiService.ExecuteWithTokenRefreshAsync(
                    token => _client.Content.Post.CreatePost(request, token, cancellationToken)
                );
    }
}