namespace Observatorio.API.Filters;

public class ApiKeyAuthFilter : IAsyncActionFilter
{
    private const string API_KEY_HEADER = "X-API-Key";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Obtener API Key del header
        if (!context.HttpContext.Request.Headers.TryGetValue(API_KEY_HEADER, out var extractedApiKey))
        {
            // Si no hay API Key, verificar si la ruta requiere autenticación JWT
            if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    error = "Unauthorized",
                    message = "API Key or JWT Token is required"
                });
                return;
            }
            
            await next();
            return;
        }

        // Validar API Key
        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
        var isValid = await userService.ValidateApiKeyAsync(extractedApiKey.ToString());

        if (!isValid)
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                error = "Unauthorized",
                message = "Invalid API Key"
            });
            return;
        }

        await next();
    }
}