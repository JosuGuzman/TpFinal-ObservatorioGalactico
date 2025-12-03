namespace Observatorio.API.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IWebHostEnvironment env)
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
            _logger.LogError(ex, "Error no manejado en la API");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var responseObj = new ApiResponse<object>
        {
            Success = false,
            Timestamp = DateTime.UtcNow
        };

        switch (exception)
        {
            case ValidationException validationEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                responseObj.Message = validationEx.Message;
                break;

            case NotFoundException notFoundEx:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                responseObj.Message = notFoundEx.Message;
                break;

            case UnauthorizedAccessException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                responseObj.Message = "Acceso no autorizado";
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                responseObj.Message = _env.IsDevelopment() 
                    ? exception.Message 
                    : "Se produjo un error interno del servidor";
                
                if (_env.IsDevelopment())
                {
                    responseObj.Data = new
                    {
                        exception.StackTrace,
                        exception.Source,
                        InnerException = exception.InnerException?.Message
                    };
                }
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(responseObj, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(jsonResponse);
    }
}