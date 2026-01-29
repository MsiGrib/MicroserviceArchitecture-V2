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

                    if (!serviceConfig.Controllers.TryGetValue("Auth", out var controllerConfig))
                        throw new ArgumentException("Auth controller not found in Identity service configuration");

                    _authEndpoints = new AuthEndpoints(_client, controllerConfig.BasePath);
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

                    if (!serviceConfig.Controllers.TryGetValue("User", out var controllerConfig))
                        throw new ArgumentException("User controller not found in Identity service configuration");

                    _userEndpoints = new UserEndpoints(_client, controllerConfig.BasePath);
                }

                return _userEndpoints;
            }
        }
    }
}