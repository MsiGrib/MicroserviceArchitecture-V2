namespace BLL.Integrations.Kafka.Outbox.Events
{
    public class UserLoggedInEvent
    {
        public Guid UserId { get; set; }
        public DateTime LoggedInAt { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public string TimeZone { get; set; } = string.Empty;
        public string LoginType { get; set; } = string.Empty;
    }
}