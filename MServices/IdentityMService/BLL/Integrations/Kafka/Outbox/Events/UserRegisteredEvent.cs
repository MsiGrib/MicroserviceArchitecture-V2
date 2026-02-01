namespace BLL.Integrations.Kafka.Outbox.Events
{
    public record UserRegisteredEvent
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime RegisteredAt { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public string TimeZone { get; set; } = string.Empty;
    }
}