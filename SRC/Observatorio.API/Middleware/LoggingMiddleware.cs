namespace Observatorio.API.Middleware;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;
        
        // Log de la solicitud entrante
        _logger.LogInformation("Incoming request: {Method} {Path} from {RemoteIp}", 
            context.Request.Method, 
            context.Request.Path, 
            context.Connection.RemoteIpAddress);

        // Copiar el stream del cuerpo para poder leerlo sin afectar el pipeline
        context.Request.EnableBuffering();

        using (var requestBodyStream = new MemoryStream())
        {
            await context.Request.Body.CopyToAsync(requestBodyStream);
            requestBodyStream.Seek(0, SeekOrigin.Begin);
            
            var requestBodyText = await new StreamReader(requestBodyStream).ReadToEndAsync();
            if (!string.IsNullOrEmpty(requestBodyText))
            {
                _logger.LogDebug("Request body: {RequestBody}", requestBodyText);
            }

            context.Request.Body.Seek(0, SeekOrigin.Begin);
        }

        // Capturar la respuesta
        var originalBodyStream = context.Response.Body;
        using (var responseBodyStream = new MemoryStream())
        {
            context.Response.Body = responseBodyStream;

            await _next(context);

            var endTime = DateTime.UtcNow;
            var duration = endTime - startTime;

            responseBodyStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();
            responseBodyStream.Seek(0, SeekOrigin.Begin);

            // Log de la respuesta
            _logger.LogInformation("Response: {StatusCode} in {Duration}ms for {Method} {Path}",
                context.Response.StatusCode,
                duration.TotalMilliseconds,
                context.Request.Method,
                context.Request.Path);

            if (context.Response.StatusCode >= 400 && !string.IsNullOrEmpty(responseBody))
            {
                _logger.LogWarning("Error response body: {ResponseBody}", responseBody);
            }

            await responseBodyStream.CopyToAsync(originalBodyStream);
        }
    }
}