namespace Observatorio.Core.Interfaces;

public interface ISystemLogRepository : IRepository<SystemLog>
{
    Task<IEnumerable<SystemLog>> GetByUserAsync(int userId);
    Task<IEnumerable<SystemLog>> GetByEventTypeAsync(string eventType);
    Task<IEnumerable<SystemLog>> GetRecentAsync(int limit);
    Task ClearOldLogsAsync(DateTime olderThan);
}