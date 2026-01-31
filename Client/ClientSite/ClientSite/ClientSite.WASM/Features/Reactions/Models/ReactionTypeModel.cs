namespace ClientSite.WASM.Features.Reactions.Models
{
    public record ReactionTypeModel
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Emoji { get; init; } = string.Empty;
        public string Color { get; init; } = string.Empty;
    }
}