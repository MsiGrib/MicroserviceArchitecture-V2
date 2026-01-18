using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ApiGateway
{
    public class GatewayHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
            => Task.FromResult(HealthCheckResult.Healthy("Gateway is running"));
    }
}