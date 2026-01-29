namespace Api.Models.MServices.ContentMService.Endpoints.PostEndpoints.Requests
{
    public record CreatePostRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}