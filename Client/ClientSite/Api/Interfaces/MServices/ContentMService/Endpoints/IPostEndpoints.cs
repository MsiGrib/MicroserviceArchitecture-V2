using Api.Models.MServices.ContentMService.Endpoints.PostEndpoints.Requests;
using Api.Models.MServices.ContentMService.Endpoints.PostEndpoints.Responses;

namespace Api.Interfaces.MServices.ContentMService.Endpoints
{
    public interface IPostEndpoints
    {
        public Task<List<PostDTO>?> GetAllPosts(CancellationToken cancellationToken = default);
        public Task<PostDTO> GetPost(Guid Id, CancellationToken cancellationToken = default);
        public Task<Guid> CreatePost(CreatePostRequest request, string token, CancellationToken cancellationToken = default);
        public Task<PostDTO> UpdatePost(Guid id, UpdatePostRequest request, string token, CancellationToken cancellationToken = default);
        public Task DeletePost(Guid id, string token, CancellationToken cancellationToken = default);
    }
}