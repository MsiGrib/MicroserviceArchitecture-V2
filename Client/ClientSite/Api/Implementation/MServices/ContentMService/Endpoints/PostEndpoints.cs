using Api.Interfaces;
using Api.Interfaces.MServices.ContentMService.Endpoints;
using Api.Models.MServices.ContentMService.Endpoints.PostEndpoints.Requests;
using Api.Models.MServices.ContentMService.Endpoints.PostEndpoints.Responses;
using RestSharp;

namespace Api.Implementation.MServices.ContentMService.Endpoints
{
    internal class PostEndpoints : BaseEndPoint, IPostEndpoints
    {
        public PostEndpoints(IMicroservicesClient client, string basePath)
            : base(client, "Content", basePath) { }

        public Task<PostDTO?> GetPost(Guid id, CancellationToken cancellationToken = default)
        {
            var restRequest = new RestRequest(BuildUrl($"/{id}"), Method.Get);

            return ExecuteAsync<PostDTO?>(restRequest, ctn: cancellationToken);
        }

        public Task<List<PostDTO>?> GetAllPosts(CancellationToken cancellationToken = default)
        {
            var restRequest = new RestRequest(BuildUrl(""), Method.Get);

            return ExecuteAsync<List<PostDTO>?>(restRequest, ctn: cancellationToken);
        }

        public Task<Guid> CreatePost(CreatePostRequest request, string token, CancellationToken cancellationToken = default)
        {
            var restRequest = new RestRequest(BuildUrl(""), Method.Post);

            restRequest.AddJsonBody(request);

            return ExecuteAsync<Guid>(restRequest, token, ctn: cancellationToken);
        }

        public Task<PostDTO?> UpdatePost(Guid id, UpdatePostRequest request, string token, CancellationToken cancellationToken = default)
        {
            var restRequest = new RestRequest(BuildUrl($"/{id}"), Method.Put);

            restRequest.AddJsonBody(request);

            return ExecuteAsync<PostDTO?>(restRequest, token, ctn: cancellationToken);
        }

        public Task DeletePost(Guid id, string token, CancellationToken cancellationToken = default)
        {
            var restRequest = new RestRequest(BuildUrl($"/{id}"), Method.Delete);

            return ExecuteAsync(restRequest, token, ctn: cancellationToken);
        }
    }
}