using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace BLL.Integrations
{
    public class IntegrationAuthHandler : DelegatingHandler
    {
        private readonly IMemoryCache _cache;
        private readonly string _apiKey;

        public IntegrationAuthHandler(IMemoryCache cache, IConfiguration configuration)
        {
            _cache = cache;
            _apiKey = configuration["Integration:IdentityService:ApiKey"] ?? string.Empty;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Remove("X-Service-Api-Key");
            request.Headers.Add("X-Service-Api-Key", _apiKey);

            if (request.Method == HttpMethod.Get)
            {
                var cacheKey = GenerateCacheKey(request);
                if (_cache.TryGetValue(cacheKey, out HttpResponseMessage? cachedResponse))
                    return cachedResponse!;
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (request.Method == HttpMethod.Get && response.IsSuccessStatusCode)
            {
                var cacheKey = GenerateCacheKey(request);
                _cache.Set(cacheKey, response, TimeSpan.FromMinutes(5));
            }

            return response;
        }

        private string GenerateCacheKey(HttpRequestMessage request)
            => $"http_{request.Method}_{request.RequestUri}";
    }
}