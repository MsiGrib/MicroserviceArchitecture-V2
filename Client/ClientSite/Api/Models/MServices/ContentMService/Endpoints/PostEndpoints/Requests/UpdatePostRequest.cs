namespace Api.Models.MServices.ContentMService.Endpoints.PostEndpoints.Requests
{
    public record UpdatePostRequest
    {
        public required string Title { get; init; }
        public required string Content { get; init; }
    }
}