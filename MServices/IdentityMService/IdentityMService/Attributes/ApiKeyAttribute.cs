using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IdentityMService.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        private const string API_KEY_HEADER = "X-Service-Api-Key";
        private const string CONFIG_PATH = "Services:ContentService:ApiKey";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

            var apiKey = context.HttpContext.Request.Headers[API_KEY_HEADER].FirstOrDefault();
            var validApiKey = configuration[CONFIG_PATH];

            if (string.IsNullOrEmpty(apiKey) || apiKey != validApiKey)
            {
                context.Result = new UnauthorizedObjectResult(new { error = "Invalid or missing API key" });
                return;
            }

            await next();
        }
    }
}