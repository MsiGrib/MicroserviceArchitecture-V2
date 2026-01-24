using Api.Implementation.MServices.IdentityMService.Endpoints;
using Api.Interfaces;
using Api.Interfaces.MServices.IdentityMService;
using Api.Interfaces.MServices.IdentityMService.Endpoints;
using Api.Models;
using Microsoft.Extensions.Options;

namespace Api.Implementation.MServices.IdentityMService
{
    internal class IdentityMService : IIdentityMService
    {
        private readonly IMicroservicesClient _client;
        private readonly Configuration _configuration;

        private IAuthEndpoints? _authEndpoints;
        private IUserEndpoints? _userEndpoints;

        public IdentityMService(IMicroservicesClient client, IOptions<Configuration> configuration)
        {
            _client = client;
            _configuration = configuration.Value;
        }

        public IAuthEndpoints Auth
        {
            get
            {
                if (_authEndpoints == null)
                {
                    if (!_configuration.Services.TryGetValue("Identity", out var serviceConfig))
                        throw new ArgumentException("Identity service not found in configuration");

                    _authEndpoints = new AuthEndpoints(_client, serviceConfig.AuthBasePath);
                }

                return _authEndpoints;
            }
        }

        public IUserEndpoints User
        {
            get
            {
                if (_userEndpoints == null)
                {
                    if (!_configuration.Services.TryGetValue("Identity", out var serviceConfig))
                        throw new ArgumentException("Identity service not found in configuration");

                    _userEndpoints = new UserEndpoints(_client, serviceConfig.UserBasePath);
                }

                return _userEndpoints;
            }
        }
    }
}