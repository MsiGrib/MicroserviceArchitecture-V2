using Api.Implementation;
using Api.Interfaces;
using Api.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api
{
    public static class Configure
    {
        public static IServiceCollection AddMicroservicesIntegrationApi(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<Configuration>(
                configuration.GetSection(Configuration.ConfigurationSection));

            services.AddHttpClient(nameof(MicroservicesClient), client =>
            {
                client.Timeout = TimeSpan.FromMinutes(10);
            });

            services.AddScoped<IMicroservicesClient, MicroservicesClient>();

            return services;
        }
    }
}