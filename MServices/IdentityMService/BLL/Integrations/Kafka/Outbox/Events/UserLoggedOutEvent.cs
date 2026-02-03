namespace BLL.Integrations.Kafka.Outbox.Events
{
    public record UserLoggedOutEvent
    {
        public Guid UserId { get; init; }
        public DateTime LoggedOutAt { get; init; }
        public string LogoutType { get; init; } = string.Empty;
        public string Reason { get; init; } = string.Empty;
    }
}