using BLL.Services.Comment;
using BLL.Services.Interfaces.Comment;
using BLL.Services.Interfaces.Post;
using BLL.Services.Interfaces.Reaction;
using BLL.Services.Post;
using BLL.Services.Reaction;
using Common.Models;
using ContentMService.Middleware;
using DAL;
using DAL.Repositories.Comment;
using DAL.Repositories.Interfaces.Comment;
using DAL.Repositories.Interfaces.Post;
using DAL.Repositories.Interfaces.Reaction;
using DAL.Repositories.Post;
using DAL.Repositories.Reaction;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

namespace ContentMService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Builder

            #region Main

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

            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
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
                        "http://localhost:5030",   // Blazor клиент
                        "https://localhost:5001",  // Identity Service
                        "http://localhost:5000"    // Identity Service
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

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Gateway";
                options.DefaultChallengeScheme = "Gateway";
            })
            .AddScheme<AuthenticationSchemeOptions, GatewayAuthenticationHandler>("Gateway", options => { });

            builder.Services.AddHttpContextAccessor();

            #endregion

            #region Additionally

            builder.Services.AddScoped<IPostRepository, PostRepository>();
            builder.Services.AddScoped<ICommentRepository, CommentRepository>();
            builder.Services.AddScoped<IReactionRepository, ReactionRepository>();

            builder.Services.AddScoped<IPostService, PostService>();
            builder.Services.AddScoped<ICommentService, CommentService>();
            builder.Services.AddScoped<IReactionService, ReactionService>();

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

            app.UseMiddleware<GatewayAuthMiddleware>();

            app.UseHttpsRedirection();

            app.MapControllers();

            app.Lifetime.ApplicationStarted.Register(() =>
            {
                var logger = app.Services.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("Content Service запущен!");
                logger.LogInformation("Swagger UI доступен по адресу: {SwaggerUrl}",
                    "https://localhost:5003/swagger или http://localhost:5002/swagger");
                logger.LogInformation("API доступен по адресу: {ApiUrl}",
                    "https://localhost:5003/api или http://localhost:5002/api");
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