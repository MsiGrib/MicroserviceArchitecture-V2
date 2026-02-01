namespace BLL.Integrations.Kafka
{
    public interface IKafkaProducer
    {
        public Task<bool> ProduceAsync(string topic, string message, string? eventType = null);
        public Task DisconnectAsync();
    }
}