namespace ClientSite.WASM.Features.Reactions.Models
{
    public record ReactionGroup
    {
        public int Type { get; init; }
        public int Count { get; init; }
        public bool HasUserReaction { get; init; }
        public List<Guid> UserIds { get; init; } = new();
    }
}