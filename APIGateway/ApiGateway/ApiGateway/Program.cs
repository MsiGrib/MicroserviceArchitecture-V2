using ApiGateway.Configurations;
using ApiGateway.Extensions;
using ApiGateway.Middleware;

namespace ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Configuration

            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            #endregion

            #region Services Registration

            builder.Services.AddCustomCors();
            builder.Services.AddSwagger();
            builder.Services.AddCustomAuthentication(builder.Configuration);
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddGatewayServices();
            builder.Services.AddReverseProxyWithTransforms();
            builder.Services.AddHealthChecks();
            builder.Services.AddControllers();

            #endregion

            var app = builder.Build();

            #region Middleware Pipeline

            app.UseSwaggerUI();
            app.UseCustomCors();
            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapHealthChecks("/health");
            app.MapControllers();
            app.UseMiddleware<UnauthorizedMiddleware>();
            app.MapReverseProxy();
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            #endregion

            app.Run();
        }
    }
}