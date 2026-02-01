using BLL.Services.Interfaces.UserAnalytics;
using DAL.Repositories.Interfaces.LoginStatistic;
using DAL.Repositories.Interfaces.LogoutStatistic;
using DAL.Repositories.Interfaces.RegistrationStatistic;

namespace BLL.Services.UserAnalytics
{
    public class UserAnalyticsService : IUserAnalyticsService
    {
        private readonly IRegistrationStatisticRepository _registrationRepository;
        private readonly ILoginStatisticRepository _loginRepository;
        private readonly ILogoutStatisticRepository _logoutRepository;

        public UserAnalyticsService(IRegistrationStatisticRepository registrationRepository,
            ILoginStatisticRepository loginRepository, ILogoutStatisticRepository logoutRepository)
        {
            _registrationRepository = registrationRepository;
            _loginRepository = loginRepository;
            _logoutRepository = logoutRepository;
        }
    }
}