namespace ApiGateway.Middleware
{
    public class UnauthorizedMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UnauthorizedMiddleware> _logger;

        public UnauthorizedMiddleware(RequestDelegate next, ILogger<UnauthorizedMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            try
            {
                using var responseBody = new MemoryStream();
                context.Response.Body = responseBody;

                await _next(context);

                context.Response.Body = originalBodyStream;

                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("Response already started, cannot modify 401 response");
                    return;
                }

                if (context.Response.StatusCode == 401)
                {
                    _logger.LogInformation("Handling 401 Unauthorized response");

                    context.Response.Body = originalBodyStream;
                    context.Response.Clear();

                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = 401;

                    var response = new
                    {
                        error = "Unauthorized",
                        message = "Invalid or expired token",
                        timestamp = DateTime.UtcNow
                    };

                    var json = System.Text.Json.JsonSerializer.Serialize(response);
                    await context.Response.WriteAsync(json);
                }
                else
                {
                    responseBody.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UnauthorizedMiddleware");
                context.Response.Body = originalBodyStream;
                throw;
            }
        }
    }
}