using ApiGateway.Services.Interfaces;
using Yarp.ReverseProxy.Transforms;

namespace ApiGateway.Services
{
    public static class HeaderTransformService
    {
        public static async ValueTask ApplyTransform(RequestTransformContext context)
        {
            var headerService = context.HttpContext.RequestServices.GetRequiredService<IGatewayHeaderService>();
            await headerService.TransformRequestAsync(context.HttpContext, context.ProxyRequest);
        }
    }
}