namespace WatchTower.Core.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<UserResponse?> GetUserProfileAsync(int userId);
    Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    Task<bool> ResetPasswordAsync(string email);
}