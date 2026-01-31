namespace Api.Models.MServices.ContentMService.Endpoints.PostEndpoints.Requests
{
    public record CreatePostRequest
    {
        public required string Title { get; init; }
        public required string Content { get; init; }
    }
}