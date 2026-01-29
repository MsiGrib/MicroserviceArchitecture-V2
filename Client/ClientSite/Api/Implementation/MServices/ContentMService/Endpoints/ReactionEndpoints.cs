using Api.Interfaces;
using Api.Interfaces.MServices.ContentMService.Endpoints;
using Api.Models.MServices.ContentMService.Endpoints.ReactionEndpoints.Requests;
using Api.Models.MServices.ContentMService.Endpoints.ReactionEndpoints.Responses;
using RestSharp;

namespace Api.Implementation.MServices.ContentMService.Endpoints
{
    internal class ReactionEndpoints : BaseEndPoint, IReactionEndpoints
    {
        public ReactionEndpoints(IMicroservicesClient client, string basePath)
            : base(client, "Content", basePath) { }

        public Task<List<ReactionDTO>> GetReactionsByPost(Guid postId, CancellationToken cancellationToken = default)
        {
            var restRequest = new RestRequest(BuildUrl($"/post/{postId}"), Method.Get);
            return ExecuteAsync<List<ReactionDTO>>(restRequest, ctn: cancellationToken);
        }

        public Task<ReactionDTO> AddOrUpdateReaction(AddReactionRequest request, string token, CancellationToken cancellationToken = default)
        {
            var restRequest = new RestRequest(BuildUrl(""), Method.Post);
            restRequest.AddJsonBody(request);
            return ExecuteAsync<ReactionDTO>(restRequest, token, ctn: cancellationToken);
        }

        public Task RemoveReaction(Guid postId, string token, CancellationToken cancellationToken = default)
        {
            var restRequest = new RestRequest(BuildUrl($"/post/{postId}"), Method.Delete);
            return ExecuteAsync(restRequest, token, ctn: cancellationToken);
        }
    }
}