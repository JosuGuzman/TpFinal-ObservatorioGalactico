namespace Observatorio.Core.Interfaces;

public interface IExplorationHistoryRepository : IRepository<ExplorationHistory>
{
    Task<IEnumerable<ExplorationHistory>> GetByUserAsync(int userId, int limit = 50);
    Task<IEnumerable<ExplorationHistory>> GetRecentByUserAsync(int userId, DateTime since);
    Task<int> ClearUserHistoryAsync(int userId);
    Task<IEnumerable<ExplorationHistory>> GetByObjectAsync(string objectType, int objectId);
}