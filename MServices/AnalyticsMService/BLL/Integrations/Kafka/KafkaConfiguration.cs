namespace BLL.Integrations.Kafka
{
    public record KafkaConfiguration
    {
        public string BootstrapServers { get; set; } = string.Empty;
        public string IdentityEventsTopic { get; set; } = string.Empty;
        public string ContentEventsTopic { get; set; } = string.Empty;
        public string ConsumerGroup { get; set; } = string.Empty;
    }
}