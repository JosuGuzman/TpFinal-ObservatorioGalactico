using WatchTower.API.Models.DTOs;

namespace WatchTower.API.Services;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<bool> RegisterAsync(RegisterRequest request);
    Task<UserResponse?> GetUserProfileAsync(int userId);
}