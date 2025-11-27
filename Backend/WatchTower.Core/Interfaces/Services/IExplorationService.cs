namespace WatchTower.Core.Interfaces.Services;

public interface IExplorationService
{
    Task<IEnumerable<ExplorationHistory>> GetUserHistoryAsync(int userId);
    Task<bool> AddToHistoryAsync(ExplorationHistory history);
    Task<bool> UpdateTimeSpentAsync(int historyId, int timeSpent, int userId);
    Task<int> GetTotalExplorationTimeAsync(int userId);
    Task<int> GetUniqueBodiesExploredCountAsync(int userId);
    Task<bool> RecordExplorationAsync(int userId, int celestialBodyId, int timeSpentSeconds = 60);
}