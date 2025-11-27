namespace WatchTower.Core.Interfaces.Repositories;

public interface IExplorationRepository
{
    Task<IEnumerable<ExplorationHistory>> GetUserHistoryAsync(int userId);
    Task<bool> AddToHistoryAsync(ExplorationHistory history);
    Task<bool> UpdateTimeSpentAsync(int historyId, int timeSpent);
    Task<int> GetTotalExplorationTimeAsync(int userId);
    Task<int> GetUniqueBodiesExploredCountAsync(int userId); // Este método faltaba
}