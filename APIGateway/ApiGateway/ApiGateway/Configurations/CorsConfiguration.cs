namespace ApiGateway.Configurations
{
    public static class CorsConfiguration
    {
        public static IServiceCollection AddCustomCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("BlazorClient", policy =>
                {
                    policy.WithOrigins(
                            "https://localhost:7162",
                            "http://localhost:5030",
                            "https://localhost:7010",
                            "http://localhost:5010"
                        )
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .WithExposedHeaders("Location");
                });

                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            return services;
        }

        public static IApplicationBuilder UseCustomCors(this IApplicationBuilder app)
        {
            app.UseCors("BlazorClient");
            return app;
        }
    }
}