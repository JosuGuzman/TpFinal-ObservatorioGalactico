namespace Observatorio.Core.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User> GetByEmailAsync(string email);
    Task<User> GetByUsernameAsync(string username);
    Task<User> GetByApiKeyAsync(string apiKey);
    Task<bool> EmailExistsAsync(string email);
    Task<bool> UsernameExistsAsync(string username);
    Task UpdateLastLoginAsync(int userId);
    Task<IEnumerable<User>> GetByRoleAsync(int roleId);
    Task DeactivateUserAsync(int userId);
    Task ActivateUserAsync(int userId);
}