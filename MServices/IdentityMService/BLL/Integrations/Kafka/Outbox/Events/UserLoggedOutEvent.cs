namespace BLL.Integrations.Kafka.Outbox.Events
{
    public class UserLoggedOutEvent
    {
        public Guid UserId { get; set; }
        public DateTime LoggedOutAt { get; set; }
        public string LogoutType { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }
}