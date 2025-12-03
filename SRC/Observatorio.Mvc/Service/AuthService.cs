using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Observatorio.Core.Interfaces;
using System.Security.Claims;

namespace Observatorio.Mvc.Services;

public interface IAuthService
{
    Task<bool> LoginAsync(string email, string password, bool rememberMe);
    Task LogoutAsync();
    Task<bool> RegisterAsync(string email, string username, string password);
    bool IsAuthenticated();
    int GetCurrentUserId();
    string GetCurrentUserRole();
}

public class AuthService : IAuthService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserService _userService;

    public AuthService(IHttpContextAccessor httpContextAccessor, IUserService userService)
    {
        _httpContextAccessor = httpContextAccessor;
        _userService = userService;
    }

    public async Task<bool> LoginAsync(string email, string password, bool rememberMe)
    {
        try
        {
            var user = await _userService.AuthenticateAsync(email, password);
            
            if (user == null || !user.IsActive)
                return false;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role.RoleName)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            await _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    public async Task<bool> RegisterAsync(string email, string username, string password)
    {
        try
        {
            await _userService.RegisterAsync(email, username, password, 2); // Default role: User
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool IsAuthenticated()
    {
        return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }

    public int GetCurrentUserId()
    {
        var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null && int.TryParse(claim.Value, out var userId) ? userId : 0;
    }

    public string GetCurrentUserRole()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value ?? "Guest";
    }
}