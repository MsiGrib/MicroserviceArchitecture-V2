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

        protected BaseEndPoint(IMicroservicesClient client, string serviceName, string? customBasePath = null)
        {
            var internalClient = (IMicroservicesClientInternal)client;
            Settings = internalClient.Configuration;

            if (!Settings.Services.TryGetValue(serviceName, out var serviceConfig))
                throw new ArgumentException($"Service '{serviceName}' not found in configuration");

            ServiceBasePath = customBasePath!;

            var httpClient = internalClient.HttpClientFactory.CreateClient(nameof(MicroservicesClient));
            httpClient.Timeout = TimeSpan.FromMinutes(10);

            RestClient = new RestClient(httpClient);
        }

        protected virtual async Task<T> ExecuteAsync<T>(RestRequest request, string? token = null, CancellationToken ctn = default)
        {
            if (!string.IsNullOrWhiteSpace(token))
                request.Authenticator = new JwtAuthenticator(token);

            var response = await RestClient.ExecuteAsync(request, ctn);

            return response switch
            {
                { IsSuccessStatusCode: true } when !string.IsNullOrEmpty(response.Content) =>
                    JsonSerializer.Deserialize<T>(response.Content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!,
                { IsSuccessStatusCode: true } =>
                    default(T)!,
                { StatusCode: HttpStatusCode.BadRequest } =>
                    throw new Exception($"Bad Request: {response.Content}"),
                { StatusCode: HttpStatusCode.Unauthorized } =>
                    throw new UnauthorizedAccessException("Authentication failed"),
                { StatusCode: HttpStatusCode.NotFound } =>
                    throw new Exception($"Resource not found: {response.Content}"),
                { StatusCode: HttpStatusCode.Forbidden } =>
                    throw new Exception($"Access forbidden: {response.Content}"),
                _ => throw new Exception(
                    $"Request failed: {response.StatusCode}, Content: {response.Content}")
            };
        }

        protected virtual async Task ExecuteAsync(RestRequest request, string? token = null, CancellationToken ctn = default)
        {
            if (!string.IsNullOrWhiteSpace(token))
                request.Authenticator = new JwtAuthenticator(token);

            var response = await RestClient.ExecuteAsync(request, ctn);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Request failed: {response.StatusCode}, Content: {response.Content}");
        }

        protected string BuildUrl(string endpoint) =>
            $"{Settings.ApiGatewayBaseUrl}{ServiceBasePath}{endpoint}";
    }
}