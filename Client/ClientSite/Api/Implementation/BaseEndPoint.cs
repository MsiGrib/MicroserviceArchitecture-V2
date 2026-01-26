using Api.Interfaces;
using Api.Models;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;
using System.Text.Json;

namespace Api.Implementation
{
    internal abstract class BaseEndPoint
    {
        protected readonly RestClient RestClient;
        protected readonly Configuration Settings;
        protected readonly string ServiceBasePath;
        private readonly Func<Task<string?>> _getTokenCallback;
        private readonly Func<Task<bool>> _refreshTokenCallback;

        protected BaseEndPoint(IMicroservicesClient client,
            string serviceName, string? customBasePath = null,
            Func<Task<string?>>? getTokenCallback = null, Func<Task<bool>>? refreshTokenCallback = null)
        {
            var internalClient = (IMicroservicesClientInternal)client;
            Settings = internalClient.Configuration;

            if (!Settings.Services.TryGetValue(serviceName, out var serviceConfig))
                throw new ArgumentException($"Service '{serviceName}' not found in configuration");

            ServiceBasePath = customBasePath!;
            _getTokenCallback = getTokenCallback;
            _refreshTokenCallback = refreshTokenCallback;

            var httpClient = internalClient.HttpClientFactory.CreateClient(nameof(MicroservicesClient));
            httpClient.Timeout = TimeSpan.FromMinutes(10);

            RestClient = new RestClient(httpClient);
        }

        protected virtual async Task<T> ExecuteAsync<T>(RestRequest request, string? token = null, bool allowRetry = true,
            CancellationToken ctn = default)
        {
            var maxRetries = 1;

            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                try
                {
                    var currentToken = token ?? (_getTokenCallback != null ? await _getTokenCallback() : null);

                    if (!string.IsNullOrWhiteSpace(currentToken))
                        request.Authenticator = new JwtAuthenticator(currentToken);

                    var response = await RestClient.ExecuteAsync(request, ctn);

                    if (response.IsSuccessStatusCode)
                    {
                        return !string.IsNullOrEmpty(response.Content)
                            ? JsonSerializer.Deserialize<T>(
                                response.Content,
                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!
                            : default(T)!;
                    }

                    if (response.StatusCode == HttpStatusCode.Unauthorized &&
                        allowRetry && attempt < maxRetries && _refreshTokenCallback != null)
                    {
                        var refreshSuccess = await _refreshTokenCallback();
                        if (!refreshSuccess)
                            throw new UnauthorizedAccessException("Token refresh failed");

                        continue;
                    }

                    throw CreateException(response);
                }
                catch (UnauthorizedAccessException) when (attempt < maxRetries && _refreshTokenCallback != null)
                {
                    var refreshSuccess = await _refreshTokenCallback();
                    if (!refreshSuccess)
                        throw;
                }
            }

            throw new UnauthorizedAccessException("Authentication failed");
        }

        protected virtual async Task ExecuteAsync(RestRequest request, string? token = null, bool allowRetry = true,
                CancellationToken ctn = default)
            => await ExecuteAsync<object?>(request, token, allowRetry, ctn);

        private Exception CreateException(RestResponse response)
        {
            return response.StatusCode switch
            {
                HttpStatusCode.BadRequest =>
                    new Exception($"Bad Request: {response.Content}"),
                HttpStatusCode.Unauthorized =>
                    new UnauthorizedAccessException("Authentication failed"),
                HttpStatusCode.Forbidden =>
                    new Exception($"Access forbidden: {response.Content}"),
                HttpStatusCode.NotFound =>
                    new Exception($"Resource not found: {response.Content}"),
                _ => new Exception($"Request failed: {response.StatusCode}, Content: {response.Content}")
            };
        }

        protected string BuildUrl(string endpoint)
            => $"{Settings.ApiGatewayBaseUrl}{ServiceBasePath}{endpoint}";
    }
}