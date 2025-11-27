namespace WatchTower.Core.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int userId);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<int> CreateAsync(User user);
    Task UpdateAsync(User user);
    Task<bool> UserExistsAsync(string username, string email);
    Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role);
    Task<bool> DeactivateUserAsync(int userId);
    Task UpdateLastLoginAsync(int userId);
}
