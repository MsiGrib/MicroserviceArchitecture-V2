using Api.Interfaces;
using Api.Interfaces.MServices.ContentMService.Endpoints;
using Api.Models.MServices.ContentMService.Endpoints.CommentEndpoints.Requests;
using Api.Models.MServices.ContentMService.Endpoints.CommentEndpoints.Responses;
using RestSharp;

namespace Api.Implementation.MServices.ContentMService.Endpoints
{
    internal class CommentEndpoints : BaseEndPoint, ICommentEndpoints
    {
        public CommentEndpoints(IMicroservicesClient client, string basePath)
            : base(client, "Content", basePath) { }

        public Task<List<CommentDTO>> GetCommentsByPost(Guid postId, CancellationToken cancellationToken = default)
        {
            var restRequest = new RestRequest(BuildUrl($"/post/{postId}"), Method.Get);
            return ExecuteAsync<List<CommentDTO>>(restRequest, ctn: cancellationToken);
        }

        public Task<Guid> CreateComment(CreateCommentRequest request, string token, CancellationToken cancellationToken = default)
        {
            var restRequest = new RestRequest(BuildUrl(""), Method.Post);
            restRequest.AddJsonBody(request);
            return ExecuteAsync<Guid>(restRequest, token, ctn: cancellationToken);
        }

        public Task<Guid> DeleteComment(Guid id, string token, CancellationToken cancellationToken = default)
        {
            var restRequest = new RestRequest(BuildUrl($"/{id}"), Method.Delete);
            return ExecuteAsync<Guid>(restRequest, token, ctn: cancellationToken);
        }
    }
}