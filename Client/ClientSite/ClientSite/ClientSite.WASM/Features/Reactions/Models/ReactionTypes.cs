namespace ClientSite.WASM.Features.Reactions.Models
{
    public static class ReactionTypes
    {
        public static readonly List<ReactionTypeModel> All = new()
        {
            new ReactionTypeModel { Id = 1, Name = "Like", Emoji = "👍", Color = "text-blue-500" },
            new ReactionTypeModel { Id = 2, Name = "Love", Emoji = "❤️", Color = "text-red-500" },
            new ReactionTypeModel { Id = 3, Name = "Haha", Emoji = "😂", Color = "text-yellow-500" },
            new ReactionTypeModel { Id = 4, Name = "Wow", Emoji = "😮", Color = "text-yellow-400" },
            new ReactionTypeModel { Id = 5, Name = "Sad", Emoji = "😢", Color = "text-blue-400" },
            new ReactionTypeModel { Id = 6, Name = "Angry", Emoji = "😠", Color = "text-red-600" },
            new ReactionTypeModel { Id = 7, Name = "Fire", Emoji = "🔥", Color = "text-orange-500" },
            new ReactionTypeModel { Id = 8, Name = "Star", Emoji = "⭐", Color = "text-yellow-300" },
            new ReactionTypeModel { Id = 9, Name = "Clap", Emoji = "👏", Color = "text-green-500" },
            new ReactionTypeModel { Id = 10, Name = "Rocket", Emoji = "🚀", Color = "text-purple-500" }
        };

        public static ReactionTypeModel? GetById(int id)
            => All.FirstOrDefault(x => x.Id == id);

        public static ReactionTypeModel? GetByEmoji(string emoji)
            => All.FirstOrDefault(x => x.Emoji == emoji);
    }
}