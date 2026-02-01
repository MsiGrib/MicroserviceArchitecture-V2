using DAL.Entities;
using DAL.Repositories.Interfaces.RegistrationStatistic;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BLL.Integrations.Kafka.EventHandlers
{
    public class UserRegisteredEventHandler : IEventHandler
    {
        private readonly IRegistrationStatisticRepository _registrationRepository;
        private readonly ILogger<UserRegisteredEventHandler> _logger;

        public UserRegisteredEventHandler(
            IRegistrationStatisticRepository registrationRepository,
            ILogger<UserRegisteredEventHandler> logger)
        {
            _registrationRepository = registrationRepository;
            _logger = logger;
        }

        public async Task HandleAsync(string message)
        {
            try
            {
                var @event = JsonConvert.DeserializeObject<UserRegisteredEvent>(message);

                DateTime? localTime = null;
                if (!string.IsNullOrEmpty(@event.TimeZone))
                {
                    try
                    {
                        var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(@event.TimeZone);
                        localTime = TimeZoneInfo.ConvertTimeFromUtc(@event.RegisteredAt, timeZoneInfo);
                    }
                    catch (TimeZoneNotFoundException)
                    {
                        _logger.LogWarning($"TimeZone not found: {@event.TimeZone}, using UTC");
                        localTime = @event.RegisteredAt.ToLocalTime();
                    }
                }
                else
                {
                    localTime = @event.RegisteredAt.ToLocalTime();
                }

                var registrationStatistic = new RegistrationStatistic
                {
                    Id = Guid.NewGuid(),
                    UserId = @event.UserId,
                    UTC = @event.RegisteredAt,
                    Local = localTime,
                    TimeZone = @event.TimeZone,
                    IpAddress = @event.IpAddress,
                    UserAgent = @event.UserAgent,
                    SourceTypeId = 1, // Web
                    StatusTypeId = 1  // Success
                };

                await _registrationRepository.AddAsync(registrationStatistic);
                await _registrationRepository.SaveChangesAsync();

                _logger.LogInformation($"Registration recorded for user: {@event.UserId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing UserRegistered event: {ex.Message}");
                throw;
            }
        }

        // Класс события (можно вынести в общую библиотеку)
        private class UserRegisteredEvent
        {
            public Guid UserId { get; set; }
            public string Email { get; set; }
            public string Username { get; set; }
            public DateTime RegisteredAt { get; set; }
            public string IpAddress { get; set; }
            public string UserAgent { get; set; }
            public string TimeZone { get; set; }
        }
    }
}