using BLL.Integrations.Kafka.Models;
using DAL.Entities;
using DAL.Repositories.Interfaces.LoginStatistic;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BLL.Integrations.Kafka.EventHandlers
{
    public class UserLoggedInEventHandler : IEventHandler
    {
        private readonly ILoginStatisticRepository _loginRepository;
        private readonly ILogger<UserLoggedInEventHandler> _logger;

        public UserLoggedInEventHandler(ILoginStatisticRepository loginRepository, ILogger<UserLoggedInEventHandler> logger)
        {
            _loginRepository = loginRepository;
            _logger = logger;
        }

        public async Task HandleAsync(string message)
        {
            try
            {
                var @event = JsonConvert.DeserializeObject<UserLoggedInEvent>(message);

                DateTime? localTime = null;
                if (!string.IsNullOrEmpty(@event.TimeZone))
                {
                    try
                    {
                        var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(@event.TimeZone);
                        localTime = TimeZoneInfo.ConvertTimeFromUtc(@event.LoggedInAt, timeZoneInfo);
                    }
                    catch (TimeZoneNotFoundException)
                    {
                        _logger.LogWarning($"TimeZone not found: {@event.TimeZone}, using UTC");
                        localTime = @event.LoggedInAt.ToLocalTime();
                    }
                }
                else
                {
                    localTime = @event.LoggedInAt.ToLocalTime();
                }

                var loginStatistic = new LoginStatistic
                {
                    Id = Guid.NewGuid(),
                    UserId = @event.UserId,
                    UTC = @event.LoggedInAt,
                    Local = localTime,
                    TimeZone = @event.TimeZone,
                    IpAddress = @event.IpAddress,
                    UserAgent = @event.UserAgent,
                    SourceTypeId = DetermineSourceTypeId(@event.LoginType),
                    StatusTypeId = 1,
                };

                await _loginRepository.AddAsync(loginStatistic);
                await _loginRepository.SaveChangesAsync();

                _logger.LogInformation($"Login recorded for user: {@event.UserId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing UserLoggedIn event: {ex.Message}");
                throw;
            }
        }

        private int DetermineSourceTypeId(string loginType)
        {
            return loginType switch
            {
                "Standard" => 1,
                "Social" => 2,
                "Biometric" => 3,
                _ => 1
            };
        }
    }
}