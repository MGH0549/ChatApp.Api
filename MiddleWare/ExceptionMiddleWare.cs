using System.Net;
using System.Text.Json;
using ChatApp.Errors;

namespace ChatApp.MiddleWare
{
    public class ExceptionMeddleWare
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMeddleWare> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMeddleWare(RequestDelegate next, ILogger<ExceptionMeddleWare> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var innerException = ex;
                while (innerException.InnerException != null)
                {
                    innerException = innerException.InnerException;
                }
                _logger.LogError(
                    "Innermost exception: {Message}",
                    innerException.Message
                );
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = _env.IsDevelopment()
                    ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace!.ToString())
                    : new ApiException(context.Response.StatusCode, ex.Message, "Internal Server Error");

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
        }
    }
}