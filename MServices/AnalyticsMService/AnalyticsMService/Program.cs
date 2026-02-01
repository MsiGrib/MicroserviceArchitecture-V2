using BLL.Integrations.Kafka;
using BLL.Integrations.Kafka.EventHandlers;
using BLL.Services.Interfaces.UserAnalytics;
using BLL.Services.UserAnalytics;
using Common.Models;
using DAL;
using DAL.Repositories;
using DAL.Repositories.Interfaces;
using DAL.Repositories.Interfaces.LoginStatistic;
using DAL.Repositories.Interfaces.LogoutStatistic;
using DAL.Repositories.Interfaces.RegistrationStatistic;
using DAL.Repositories.LoginStatistic;
using DAL.Repositories.LogoutStatistic;
using DAL.Repositories.RegistrationStatistic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.OpenApi;

namespace AnalyticsMService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Builder

            #region Main

            builder.Services.AddHttpClient();

            builder.Configuration
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            var appSettings = builder.Configuration.Get<AppSettings>();

            if (appSettings == null)
                throw new InvalidOperationException("AppSettings not configured properly");

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(appSettings.ConnectionStrings.Postgres));

            builder.Services.Configure<AppSettings>(builder.Configuration);
            builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Content Service API",
                    Version = "v1",
                    Description = "Microservice for content management"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowGateway", policy =>
                {
                    policy.WithOrigins(
                        "https://localhost:7010", // API Gateway HTTPS
                        "http://localhost:5010",   // API Gateway HTTP
                        "https://localhost:7162",  // Blazor клиент
                        "http://localhost:5030"   // Blazor клиент
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                });

                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            builder.Services.AddControllers();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddMemoryCache();

            #endregion

            #region Additionally

            builder.Services.AddScoped<ILoginStatisticRepository, LoginStatisticRepository>();
            builder.Services.AddScoped<ILogoutStatisticRepository, LogoutStatisticRepository>();
            builder.Services.AddScoped<IRegistrationStatisticRepository, RegistrationStatisticRepository>();

            builder.Services.AddScoped<IUserAnalyticsService, UserAnalyticsService>();

            builder.Services.Configure<KafkaConfiguration>(builder.Configuration.GetSection("Kafka"));

            builder.Services.AddSingleton<IKafkaConsumer, KafkaConsumer>();

            builder.Services.AddScoped<UserRegisteredEventHandler>();
            builder.Services.AddScoped<UserLoggedInEventHandler>();
            builder.Services.AddScoped<UserLoggedOutEventHandler>();

            builder.Services.AddHostedService<KafkaBackgroundService>();

            #endregion

            #endregion

            var app = builder.Build();

            #region App

            #region Main

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Content Service API v1");
                c.RoutePrefix = "swagger";
                c.DisplayRequestDuration();
            });

            app.UseCors("AllowGateway");

            app.UseHttpsRedirection();

            app.MapControllers();

            app.Lifetime.ApplicationStarted.Register(() =>
            {
                var logger = app.Services.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("Content Service запущен!");
                logger.LogInformation("Swagger UI доступен по адресу: {SwaggerUrl}",
                    "https://localhost:5005/swagger или http://localhost:5004/swagger");
                logger.LogInformation("API доступен по адресу: {ApiUrl}",
                    "https://localhost:5005/api или http://localhost:5004/api");
            });

            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    dbContext.Database.Migrate();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogInformation("База данных ContentDB успешно настроена");
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Ошибка при настройке базы данных ContentDB");
                }
            }

            #endregion

            #endregion

            app.Run();
        }
    }
}