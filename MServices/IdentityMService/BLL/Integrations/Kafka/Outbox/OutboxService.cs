using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BLL.Integrations.Kafka.Outbox
{
    public class OutboxService : IOutboxService
    {
        private readonly AppDbContext _context;
        private readonly IKafkaProducer _kafkaProducer;

        public OutboxService(AppDbContext context, IKafkaProducer kafkaProducer)
        {
            _context = context;
            _kafkaProducer = kafkaProducer;
        }

        public async Task AddEventAsync<T>(string eventType, T eventData, string topic = "identity.events", Guid? correlationId = null)
        {
            var outboxMessage = new OutboxMessage
            {
                EventType = eventType,
                EventData = JsonConvert.SerializeObject(eventData, Formatting.Indented),
                Topic = topic,
                CorrelationId = correlationId,
                CreatedAt = DateTime.UtcNow,
                Status = "Pending"
            };

            await _context.OutboxMessages.AddAsync(outboxMessage);
            // Не вызываем SaveChangesAsync здесь - это сделает вызывающий код в рамках транзакции
        }

        public async Task ProcessPendingMessagesAsync()
        {
            var pendingMessages = await _context.OutboxMessages
                .Where(m => m.Status == "Pending" && m.RetryCount < 3)
                .OrderBy(m => m.CreatedAt)
                .Take(50) // Обрабатываем по 50 сообщений за раз
                .ToListAsync();

            foreach (var message in pendingMessages)
            {
                try
                {
                    message.Status = "Processing";
                    await _context.SaveChangesAsync();

                    // Отправляем в Kafka
                    var success = await _kafkaProducer.ProduceAsync(message.Topic, message.EventData, message.EventType);

                    if (success)
                    {
                        message.Status = "Processed";
                        message.ProcessedAt = DateTime.UtcNow;
                    }
                    else
                    {
                        message.Status = "Pending";
                        message.RetryCount++;
                        message.ErrorMessage = "Failed to send to Kafka";
                    }
                }
                catch (Exception ex)
                {
                    message.Status = "Pending";
                    message.RetryCount++;
                    message.ErrorMessage = ex.Message;
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}