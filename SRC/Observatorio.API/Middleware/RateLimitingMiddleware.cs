namespace Observatorio.API.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly ConcurrentDictionary<string, List<DateTime>> _requestLog = new();
    private readonly int _maxRequests;
    private readonly int _timeWindowMinutes;

    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger, IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        
        var apiSettings = configuration.GetSection("AppSettings:ApiSettings");
        _maxRequests = apiSettings.GetValue<int>("RateLimitRequests", 100);
        _timeWindowMinutes = apiSettings.GetValue<int>("RateLimitMinutes", 1);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientIp = context.Connection.RemoteIpAddress?.ToString();
        
        if (string.IsNullOrEmpty(clientIp) || IsAdminRequest(context))
        {
            await _next(context);
            return;
        }

        var now = DateTime.UtcNow;
        var cutoff = now.AddMinutes(-_timeWindowMinutes);

        // Limpiar solicitudes antiguas
        if (_requestLog.TryGetValue(clientIp, out var requests))
        {
            requests.RemoveAll(r => r < cutoff);
        }

        // Verificar límite
        if (_requestLog.ContainsKey(clientIp) && _requestLog[clientIp].Count >= _maxRequests)
        {
            _logger.LogWarning("Rate limit exceeded for IP: {ClientIp}", clientIp);
            
            context.Response.StatusCode = 429;
            context.Response.ContentType = "application/json";
            
            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
            {
                error = "Too many requests",
                message = $"Rate limit exceeded. Maximum {_maxRequests} requests per {_timeWindowMinutes} minute(s).",
                retryAfter = $"{_timeWindowMinutes * 60} seconds"
            }));
            
            return;
        }

        // Registrar solicitud
        _requestLog.AddOrUpdate(clientIp,
            new List<DateTime> { now },
            (key, existing) =>
            {
                existing.Add(now);
                return existing;
            });

        await _next(context);
    }

    private bool IsAdminRequest(HttpContext context)
    {
        // Las rutas de administración no tienen límite de tasa
        var path = context.Request.Path.Value ?? "";
        return path.StartsWith("/api/v1/admin") || 
               path.StartsWith("/api-docs") ||
               path == "/health";
    }
}