namespace Common.Models
{
    public record RedisSettings
    {
        public string ConnectionString { get; init; } = string.Empty;
        public string InstanceName { get; init; } = "IdentityMService";
    }
}