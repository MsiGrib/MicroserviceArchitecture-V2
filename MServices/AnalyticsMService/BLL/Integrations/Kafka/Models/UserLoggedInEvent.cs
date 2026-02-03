namespace BLL.Integrations.Kafka.Models
{
    internal record UserLoggedInEvent
    {
        public Guid UserId { get; init; }
        public DateTime LoggedInAt { get; init; }
        public string IpAddress { get; init; } = string.Empty;
        public string UserAgent { get; init; } = string.Empty;
        public string TimeZone { get; init; } = string.Empty;
        public string LoginType { get; init; } = string.Empty;
    }
}