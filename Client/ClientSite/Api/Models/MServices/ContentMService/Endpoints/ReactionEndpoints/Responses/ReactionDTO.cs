namespace Api.Models.MServices.ContentMService.Endpoints.ReactionEndpoints.Responses
{
    public record ReactionDTO
    {
        public Guid Id { get; init; }
        public int Type { get; init; } = 0;
        public Guid UserId { get; init; }
        public Guid PostId { get; init; }
    }
}