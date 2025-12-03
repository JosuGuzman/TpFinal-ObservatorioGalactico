namespace Observatorio.Mvc.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Internal server error",
                    message = ex.Message,
                    requestId = context.TraceIdentifier
                });
            }
            else
            {
                context.Response.Redirect($"/Home/Error?requestId={context.TraceIdentifier}");
            }
        }
    }
}