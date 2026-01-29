namespace Api.Models.MServices.ContentMService.Endpoints.ReactionEndpoints.Requests
{
    public record AddReactionRequest
    {
        public Guid PostId { get; set; }
        public int ReactionType { get; set; }
    }
}