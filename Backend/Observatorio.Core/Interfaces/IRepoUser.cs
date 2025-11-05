using Observatorio.Core.Entities;

namespace Observatorio.Core.Interfaces;

public interface IRepoUser
{
    Task<User> GetByIdAsync(int id);
    Task<User> GetByUsernameAsync(string username);
    Task<User> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task<int> CreateAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(int id);
    Task UpdateLastLoginAsync(int userId);
}