using Api.Models.MServices.ContentMService.Endpoints.CommentEndpoints.Requests;
using Api.Models.MServices.ContentMService.Endpoints.CommentEndpoints.Responses;

namespace Api.Interfaces.MServices.ContentMService.Endpoints
{
    public interface ICommentEndpoints
    {
        public Task<List<CommentDTO>?> GetCommentsByPost(Guid postId, CancellationToken cancellationToken = default);
        public Task<Guid> CreateComment(CreateCommentRequest request, string token, CancellationToken cancellationToken = default);
        public Task<Guid> DeleteComment(Guid id, string token, CancellationToken cancellationToken = default);
    }
}