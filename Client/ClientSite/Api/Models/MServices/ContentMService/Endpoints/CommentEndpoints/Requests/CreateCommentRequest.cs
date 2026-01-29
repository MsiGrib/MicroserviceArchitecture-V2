namespace Api.Models.MServices.ContentMService.Endpoints.CommentEndpoints.Requests
{
    public record CreateCommentRequest
    {
        public Guid PostId { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}