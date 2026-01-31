namespace Api.Models.MServices.ContentMService.Endpoints.CommentEndpoints.Requests
{
    public record CreateCommentRequest
    {
        public required Guid PostId { get; init; }
        public required string Text { get; init; }
    }
}