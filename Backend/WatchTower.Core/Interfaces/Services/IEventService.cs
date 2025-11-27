namespace WatchTower.Core.Interfaces.Services;

public interface IEventService
{
    Task<EventResponse?> GetByIdAsync(int id);
    Task<IEnumerable<EventResponse>> GetUpcomingEventsAsync(int count);
    Task<IEnumerable<EventResponse>> SearchEventsAsync(EventSearchRequest request);
    Task<EventResponse> CreateAsync(EventCreateRequest request, int createdBy);
    Task<EventResponse?> UpdateAsync(int id, EventUpdateRequest request, int userId);
    Task<bool> DeactivateAsync(int eventId, int userId);
}