using Api.Implementation;
using Api.Implementation.MServices.ContentMService;
using Api.Implementation.MServices.IdentityMService;
using Api.Interfaces;
using Api.Interfaces.MServices.ContentMService;
using Api.Interfaces.MServices.IdentityMService;
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

            services.AddScoped<IIdentityMService, IdentityMService>();
            services.AddScoped<IContentMService, ContentMService>();

            return services;
        }
    }
}