namespace BLL.Integrations.Kafka.Outbox.Events
{
    public record UserRegisteredEvent
    {
        public Guid UserId { get; init; }
        public string Email { get; init; } = string.Empty;
        public string Username { get; init; } = string.Empty;
        public DateTime RegisteredAt { get; init; }
        public string IpAddress { get; init; } = string.Empty;
        public string UserAgent { get; init; } = string.Empty;
        public string TimeZone { get; init; } = string.Empty;
    }
}