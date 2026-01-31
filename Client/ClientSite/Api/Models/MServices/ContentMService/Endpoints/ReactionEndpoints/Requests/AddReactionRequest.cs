namespace Api.Models.MServices.ContentMService.Endpoints.ReactionEndpoints.Requests
{
    public record AddReactionRequest
    {
        public required Guid PostId { get; init; }
        public required int ReactionType { get; init; }
    }
}