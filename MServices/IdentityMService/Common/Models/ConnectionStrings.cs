namespace Common.Models
{
    public record ConnectionStrings
    {
        public string Postgres { get; init; } = string.Empty;
    }
}