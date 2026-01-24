using ApiGateway.Services;
using ApiGateway.Services.Interfaces;
using Yarp.ReverseProxy.Transforms;

namespace ApiGateway.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
            });

            return services;
        }

        public static IServiceCollection AddGatewayServices(this IServiceCollection services)
        {
            services.AddSingleton<IJwtService, JwtService>();
            services.AddScoped<IGatewayHeaderService, GatewayHeaderService>();

            return services;
        }

        public static IServiceCollection AddReverseProxyWithTransforms(this IServiceCollection services)
        {
            services.AddReverseProxy()
                .LoadFromConfig(services.BuildServiceProvider().GetRequiredService<IConfiguration>().GetSection("ReverseProxy"))
                .AddTransforms(transforms =>
                {
                    transforms.AddRequestTransform(HeaderTransformService.ApplyTransform);
                });

            return services;
        }
    }
}