namespace BLL.Integrations.Kafka.Outbox
{
    public interface IOutboxService
    {
        public Task AddEventAsync<T>(string eventType, T eventData, string topic = "identity.events", Guid? correlationId = null);
        public Task ProcessPendingMessagesAsync();
    }
}