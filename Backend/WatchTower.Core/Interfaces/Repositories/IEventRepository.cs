namespace WatchTower.Core.Interfaces.Repositories;

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(int id);
    Task<IEnumerable<Event>> GetUpcomingEventsAsync(int count);
    Task<IEnumerable<Event>> SearchEventsAsync(DateTime? startDate, DateTime? endDate, EventType? eventType);
    Task<int> CreateAsync(Event eventEntity);
    Task UpdateAsync(Event eventEntity);
    Task<bool> DeactivateAsync(int eventId);
}