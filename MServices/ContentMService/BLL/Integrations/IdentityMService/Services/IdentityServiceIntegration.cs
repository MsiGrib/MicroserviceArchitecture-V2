using BLL.Integrations.IdentityMService.DTOs;
using BLL.Integrations.IdentityMService.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace BLL.Integrations.IdentityMService.Services
{
    public class IdentityServiceIntegration : IIdentityServiceIntegration
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private readonly ILogger<IdentityServiceIntegration> _logger;

        private readonly string _apiKey;
        private readonly string _baseUrl;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(3);

        public IdentityServiceIntegration(IConfiguration configuration, IHttpClientFactory httpClientFactory,
            IMemoryCache cache, ILogger<IdentityServiceIntegration> logger)
        {
            _configuration = configuration;
            _cache = cache;
            _logger = logger;

            _apiKey = _configuration["Integration:IdentityService:ApiKey"]
                ?? throw new InvalidOperationException("IdentityService API Key not configured");

            _baseUrl = _configuration["Integration:IdentityService:BaseUrl"]
                ?? throw new InvalidOperationException("IdentityService BaseUrl not configured");

            _httpClient = httpClientFactory.CreateClient("IdentityService");
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.DefaultRequestHeaders.Add("X-Service-Api-Key", _apiKey);
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<UserSmallInfoDTO?> GetUserSmallInfoAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"user_small_info_{userId}";

            if (_cache.TryGetValue(cacheKey, out UserSmallInfoDTO? cachedUser))
                return cachedUser;

            try
            {
                var response = await _httpClient.GetAsync($"/api/user/small-info/{userId}", cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        _logger.LogWarning("User {UserId} not found in IdentityService", userId);
                        return null;
                    }

                    _logger.LogError("Error getting user info: {StatusCode}", response.StatusCode);
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var userInfo = JsonSerializer.Deserialize<UserSmallInfoDTO>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (userInfo != null)
                    _cache.Set(cacheKey, userInfo, _cacheDuration);

                return userInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user small info for {UserId}", userId);
                return null;
            }
        }

        public async Task<List<UserSmallInfoDTO>?> GetUsersSmallInfoAsync(List<Guid> userIds, CancellationToken cancellationToken = default)
        {
            if (userIds == null || userIds.Count == 0)
                return new List<UserSmallInfoDTO>();

            var result = new List<UserSmallInfoDTO>();
            var missingUserIds = new List<Guid>();

            foreach (var userId in userIds)
            {
                var cacheKey = $"user_small_info_{userId}";

                if (_cache.TryGetValue(cacheKey, out UserSmallInfoDTO? cachedUser) && cachedUser != null)
                    result.Add(cachedUser);
                else
                {
                    missingUserIds.Add(userId);
                    result.Add(null);
                }
            }

            if (missingUserIds.Count == 0)
                return result;

            try
            {
                var jsonContent = JsonSerializer.Serialize(missingUserIds);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/user/small-info/batch", httpContent, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error getting users info: {StatusCode}", response.StatusCode);
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var usersInfo = JsonSerializer.Deserialize<List<UserSmallInfoDTO>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (usersInfo == null)
                    return result;

                foreach (var userInfo in usersInfo)
                {
                    if (userInfo == null) continue;

                    var cacheKey = $"user_small_info_{userInfo.Id}";
                    _cache.Set(cacheKey, userInfo, _cacheDuration);

                    var index = userIds.IndexOf(userInfo.Id);
                    if (index >= 0 && index < result.Count)
                        result[index] = userInfo;
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users small info for {UserIdsCount} users", userIds.Count);
                return null;
            }
        }

        public Task<bool> ValidateApiKeyAsync(string apiKey)
            => Task.FromResult(apiKey == _apiKey);
    }
}