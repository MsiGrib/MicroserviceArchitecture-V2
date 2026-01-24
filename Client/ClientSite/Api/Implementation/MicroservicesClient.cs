using Api.Implementation.MServices.IdentityMService;
using Api.Interfaces;
using Api.Interfaces.MServices.IdentityMService;
using Api.Models;
using Microsoft.Extensions.Options;

namespace Api.Implementation
{
    public class MicroservicesClient : IMicroservicesClient, IMicroservicesClientInternal
    {
        private readonly IOptions<Configuration> _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        private IIdentityMService? _identityService;

        public MicroservicesClient(IOptions<Configuration> configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        Configuration IMicroservicesClientInternal.Configuration => _configuration.Value;
        IHttpClientFactory IMicroservicesClientInternal.HttpClientFactory => _httpClientFactory;

        public IIdentityMService Identity =>
            _identityService ??= new IdentityMService(this, _configuration);
    }
}