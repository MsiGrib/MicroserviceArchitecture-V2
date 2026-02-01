namespace BLL.Integrations.Kafka
{
    public interface IKafkaConsumer
    {
        public Task StartConsumingAsync(CancellationToken cancellationToken);
        public Task StopConsumingAsync();
    }
}