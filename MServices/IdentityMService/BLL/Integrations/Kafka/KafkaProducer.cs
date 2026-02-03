using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BLL.Integrations.Kafka
{
    public class KafkaProducer : IKafkaProducer, IDisposable
    {
        private readonly IProducer<Null, string> _producer;
        private readonly KafkaConfiguration _settings;
        private readonly ILogger<KafkaProducer> _logger;

        public KafkaProducer(IOptions<KafkaConfiguration> kafkaSettings, ILogger<KafkaProducer> logger)
        {
            _settings = kafkaSettings.Value;
            _logger = logger;

            var config = new ProducerConfig
            {
                BootstrapServers = _settings.BootstrapServers,
                Acks = Acks.All,
                MessageSendMaxRetries = 5,
                RetryBackoffMs = 3000,
                EnableIdempotence = true,
            };

            _producer = new ProducerBuilder<Null, string>(config)
                .SetErrorHandler((_, error) =>
                {
                    _logger.LogError($"Kafka Producer Error: {error.Reason}");
                }).Build();
        }

        public async Task<bool> ProduceAsync(string topic, string message, string? eventType = null)
        {
            try
            {
                var kafkaMessage = new Message<Null, string>
                {
                    Value = message,
                    Headers = new Headers()
                };

                if (!string.IsNullOrEmpty(eventType))
                {
                    kafkaMessage.Headers.Add("Event-Type", System.Text.Encoding.UTF8.GetBytes(eventType));
                }

                var deliveryResult = await _producer.ProduceAsync(topic, kafkaMessage);

                _logger.LogInformation($"Message delivered to {deliveryResult.TopicPartitionOffset}");
                return true;
            }
            catch (ProduceException<Null, string> ex)
            {
                _logger.LogError(ex, $"Delivery failed: {ex.Error.Reason}");
                return false;
            }
        }

        public async Task DisconnectAsync()
        {
            _producer.Flush(TimeSpan.FromSeconds(10));
            _producer.Dispose();
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            _producer?.Dispose();
        }
    }
}