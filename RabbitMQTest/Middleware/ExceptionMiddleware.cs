using RabbitMQTest.Errors;
using System.Net;
using System.Text.Json;

namespace RabbitMQTest.Middleware
{
    public class ExceptionMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger,
            IHostEnvironment env)
        {
            _env = env;
            _logger = logger;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                var start = DateTime.UtcNow;
                await _next(context);
                _logger.LogInformation($"Request Timing:[{DateTime.UtcNow}]");
                _logger.LogInformation($"Request Duration:{(DateTime.UtcNow - start)}ms");
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case var s when ex.Message.Contains("Could not find"):
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    case var s when ex.Message.Contains("not exist"):
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    default:
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";


                var response = new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString());

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
        }
    }
}
