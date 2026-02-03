namespace BLL.Integrations.Kafka
{
    public record KafkaConfiguration
    {
        public string BootstrapServers { get; init; } = string.Empty;
        public string IdentityEventsTopic { get; init; } = string.Empty;
        public string ContentEventsTopic { get; init; } = string.Empty;
        public string AnalyticsEventsTopic { get; init; } = string.Empty;
    }
}