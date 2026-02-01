using BLL.Integrations.Kafka.EventHandlers;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;

namespace BLL.Integrations.Kafka
{
    public class KafkaConsumer : IKafkaConsumer, IDisposable
    {
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly KafkaConfiguration _settings;
        private readonly ILogger<KafkaConsumer> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly string[] _topics;

        public KafkaConsumer(
            IOptions<KafkaConfiguration> kafkaSettings,
            ILogger<KafkaConsumer> logger,
            IServiceProvider serviceProvider)
        {
            _settings = kafkaSettings.Value;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _topics = new[] { _settings.IdentityEventsTopic, _settings.ContentEventsTopic };

            var config = new ConsumerConfig
            {
                BootstrapServers = _settings.BootstrapServers,
                GroupId = _settings.ConsumerGroup,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false, // Ручное подтверждение
                EnableAutoOffsetStore = false
            };

            _consumer = new ConsumerBuilder<Ignore, string>(config)
                .SetErrorHandler((_, error) =>
                {
                    _logger.LogError($"Kafka Consumer Error: {error.Reason}");
                })
                .Build();
        }

        public async Task StartConsumingAsync(CancellationToken cancellationToken)
        {
            _consumer.Subscribe(_topics);
            _logger.LogInformation($"Kafka Consumer started. Subscribed to topics: {string.Join(", ", _topics)}");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(cancellationToken);

                    if (consumeResult?.Message == null) continue;

                    var eventType = GetEventType(consumeResult.Message.Headers);

                    _logger.LogInformation($"Received event: {eventType} from topic: {consumeResult.Topic}");

                    // Обрабатываем сообщение
                    await ProcessMessageAsync(eventType, consumeResult.Message.Value, consumeResult.Topic);

                    // Подтверждаем обработку
                    _consumer.StoreOffset(consumeResult);
                    _consumer.Commit(consumeResult);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Kafka consumption cancelled");
                    break;
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, $"Error consuming message: {ex.Error.Reason}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing Kafka message");
                }
            }
        }

        private string GetEventType(Headers headers)
        {
            if (headers == null) return "Unknown";

            var eventTypeHeader = headers.FirstOrDefault(h => h.Key == "Event-Type");
            if (eventTypeHeader != null)
            {
                return Encoding.UTF8.GetString(eventTypeHeader.GetValueBytes());
            }

            return "Unknown";
        }

        private async Task ProcessMessageAsync(string eventType, string message, string topic)
        {
            using var scope = _serviceProvider.CreateScope();
            var eventHandler = GetEventHandler(eventType, scope);

            if (eventHandler != null)
            {
                try
                {
                    await eventHandler.HandleAsync(message);
                    _logger.LogDebug($"Event {eventType} processed successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error handling event {eventType}");
                    throw;
                }
            }
            else
            {
                _logger.LogWarning($"No handler found for event type: {eventType}");
            }
        }

        private IEventHandler GetEventHandler(string eventType, IServiceScope scope)
        {
            return eventType switch
            {
                "UserRegistered" => scope.ServiceProvider.GetRequiredService<UserRegisteredEventHandler>(),
                "UserLoggedIn" => scope.ServiceProvider.GetRequiredService<UserLoggedInEventHandler>(),
                "UserLoggedOut" => scope.ServiceProvider.GetRequiredService<UserLoggedOutEventHandler>(),
                _ => null
            };
        }

        public async Task StopConsumingAsync()
        {
            _consumer.Close();
            _consumer.Dispose();
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            _consumer?.Dispose();
        }
    }
}