using DAL.Entities;
using DAL.Repositories.Interfaces.LogoutStatistic;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BLL.Integrations.Kafka.EventHandlers
{
    public class UserLoggedOutEventHandler : IEventHandler
    {
        private readonly ILogoutStatisticRepository _logoutRepository;
        private readonly ILogger<UserLoggedOutEventHandler> _logger;

        public UserLoggedOutEventHandler(
            ILogoutStatisticRepository logoutRepository,
            ILogger<UserLoggedOutEventHandler> logger)
        {
            _logoutRepository = logoutRepository;
            _logger = logger;
        }

        public async Task HandleAsync(string message)
        {
            try
            {
                var @event = JsonConvert.DeserializeObject<UserLoggedOutEvent>(message);

                var logoutStatistic = new LogoutStatistic
                {
                    Id = Guid.NewGuid(),
                    UserId = @event.UserId,
                    UTC = @event.LoggedOutAt,
                    Local = @event.LoggedOutAt.ToLocalTime(),
                    SourceTypeId = DetermineSourceTypeId(@event.LogoutType),
                    StatusTypeId = 1 // Success
                };

                await _logoutRepository.AddAsync(logoutStatistic);
                await _logoutRepository.SaveChangesAsync();

                _logger.LogInformation($"Logout recorded for user: {@event.UserId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing UserLoggedOut event: {ex.Message}");
                throw;
            }
        }

        private int DetermineSourceTypeId(string logoutType)
        {
            return logoutType switch
            {
                "Manual" => 1,
                "Auto" => 2,
                "Forced" => 3,
                _ => 1
            };
        }

        private class UserLoggedOutEvent
        {
            public Guid UserId { get; set; }
            public DateTime LoggedOutAt { get; set; }
            public string LogoutType { get; set; }
            public string Reason { get; set; }
        }
    }
}