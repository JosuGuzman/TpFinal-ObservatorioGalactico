namespace Observatorio.API.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RoleAuthorizationFilter : Attribute, IAuthorizationFilter
{
    private readonly string[] _requiredRoles;

    public RoleAuthorizationFilter(params string[] roles)
    {
        _requiredRoles = roles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var userRole = context.HttpContext.User.Claims
            .FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(userRole) || !_requiredRoles.Contains(userRole))
        {
            context.Result = new ForbidResult();
        }
    }
}