namespace Common.Models
{
    public record AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; init; } = new();
    }
}