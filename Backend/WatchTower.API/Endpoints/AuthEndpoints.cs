using System.Security.Claims;
using WatchTower.API.Models.DTOs;
using WatchTower.API.Services;

namespace WatchTower.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/auth");
        
        group.MapPost("/register", async (RegisterRequest request, IAuthService authService) =>
        {
            var result = await authService.RegisterAsync(request);
            return result ? Results.Ok(new { message = "User registered successfully" }) 
                            : Results.BadRequest(new { error = "Username or email already exists" });
        });
        
        group.MapPost("/login", async (LoginRequest request, IAuthService authService) =>
        {
            var result = await authService.LoginAsync(request);
            return result != null ? Results.Ok(result) 
                                : Results.Unauthorized();
        });
        
        group.MapGet("/me", async (IAuthService authService, HttpContext context) =>
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await authService.GetUserProfileAsync(userId);
            return user != null ? Results.Ok(user) : Results.NotFound();
        }).RequireAuthorization();
    }
}