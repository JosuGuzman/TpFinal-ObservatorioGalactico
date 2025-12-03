namespace Observatorio.Core.Interfaces;

public interface ISavedSearchRepository : IRepository<SavedSearch>
{
    Task<IEnumerable<SavedSearch>> GetByUserAsync(int userId);
    Task<SavedSearch> GetByNameAsync(int userId, string name);
    Task<int> DeleteByUserAsync(int userId);
}