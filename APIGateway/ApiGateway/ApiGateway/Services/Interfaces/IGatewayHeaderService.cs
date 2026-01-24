namespace ApiGateway.Services.Interfaces
{
    public interface IGatewayHeaderService
    {
        public Task TransformRequestAsync(HttpContext context, HttpRequestMessage proxyRequest);
        public bool IsIdentityServiceEndpoint(HttpContext context);
    }
}