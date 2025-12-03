namespace Observatorio.Core.Interfaces;

public interface IEventRepository : IRepository<Event>
{
    Task<IEnumerable<Event>> GetUpcomingAsync(DateTime fromDate);
    Task<IEnumerable<Event>> GetByTypeAsync(string type);
    Task<IEnumerable<Event>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Event>> GetRecentAsync(int limit);
    Task<int> CountUpcomingAsync();
}