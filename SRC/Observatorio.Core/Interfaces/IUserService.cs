namespace Observatorio.Core.Interfaces;

public interface IUserService
{
    Task<User> AuthenticateAsync(string email, string password);
    Task<User> RegisterAsync(string email, string username, string password, int roleId = 2);
    Task<User> GetByIdAsync(int id);
    Task<User> GetByEmailAsync(string email);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(int id);
    Task ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    Task ResetPasswordAsync(string email);
    Task GenerateApiKeyAsync(int userId);
    Task<bool> ValidateApiKeyAsync(string apiKey);
    Task<bool> UserExistsAsync(string email);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<int> GetTotalUsersCountAsync();
    Task<int> GetActiveUsersCountAsync();
}