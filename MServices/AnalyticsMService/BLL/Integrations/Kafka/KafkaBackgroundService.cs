using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace BLL.Integrations.Kafka
{
    public class KafkaBackgroundService : BackgroundService
    {
        private readonly IKafkaConsumer _kafkaConsumer;
        private readonly ILogger<KafkaBackgroundService> _logger;

        public KafkaBackgroundService(IKafkaConsumer kafkaConsumer, ILogger<KafkaBackgroundService> logger)
        {
            _kafkaConsumer = kafkaConsumer;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Kafka Background Service started");

            try
            {
                await _kafkaConsumer.StartConsumingAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kafka Background Service error");
            }

            _logger.LogInformation("Kafka Background Service stopped");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Kafka Background Service");
            await _kafkaConsumer.StopConsumingAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}