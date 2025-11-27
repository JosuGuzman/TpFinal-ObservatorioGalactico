namespace WatchTower.Infrastructure.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IUserRepository _userRepository;

    public EventService(IEventRepository eventRepository, IUserRepository userRepository)
    {
        _eventRepository = eventRepository;
        _userRepository = userRepository;
    }

    public async Task<EventResponse?> GetByIdAsync(int id)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(id);
        if (eventEntity == null) return null;

        return new EventResponse
        {
            EventId = eventEntity.EventId,
            Title = eventEntity.Title,
            Description = eventEntity.Description,
            EventType = eventEntity.EventType.ToString(),
            StartDate = eventEntity.StartDate,
            EndDate = eventEntity.EndDate,
            Location = eventEntity.Location,
            Visibility = eventEntity.Visibility,
            CreatedBy = eventEntity.CreatedBy.ToString(), // Se reemplazaría con nombre real
            CreatedAt = eventEntity.CreatedAt,
            IsActive = eventEntity.IsActive
        };
    }

    public async Task<IEnumerable<EventResponse>> GetUpcomingEventsAsync(int count)
    {
        var events = await _eventRepository.GetUpcomingEventsAsync(count);
        return events.Select(e => new EventResponse
        {
            EventId = e.EventId,
            Title = e.Title,
            Description = e.Description,
            EventType = e.EventType.ToString(),
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            Location = e.Location,
            Visibility = e.Visibility,
            CreatedBy = e.CreatedBy.ToString(),
            CreatedAt = e.CreatedAt,
            IsActive = e.IsActive
        });
    }

    public async Task<IEnumerable<EventResponse>> SearchEventsAsync(EventSearchRequest request)
    {
        var events = await _eventRepository.SearchEventsAsync(request.StartDate, request.EndDate, request.EventType);
        return events.Select(e => new EventResponse
        {
            EventId = e.EventId,
            Title = e.Title,
            Description = e.Description,
            EventType = e.EventType.ToString(),
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            Location = e.Location,
            Visibility = e.Visibility,
            CreatedBy = e.CreatedBy.ToString(),
            CreatedAt = e.CreatedAt,
            IsActive = e.IsActive
        });
    }

    public async Task<EventResponse> CreateAsync(EventCreateRequest request, int createdBy)
    {
        var eventEntity = new Event
        {
            Title = request.Title,
            Description = request.Description,
            EventType = request.EventType,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Location = request.Location,
            Visibility = request.Visibility,
            CreatedBy = createdBy,
            IsActive = true
        };

        var eventId = await _eventRepository.CreateAsync(eventEntity);
        eventEntity.EventId = eventId;

        var creator = await _userRepository.GetByIdAsync(createdBy);

        return new EventResponse
        {
            EventId = eventEntity.EventId,
            Title = eventEntity.Title,
            Description = eventEntity.Description,
            EventType = eventEntity.EventType.ToString(),
            StartDate = eventEntity.StartDate,
            EndDate = eventEntity.EndDate,
            Location = eventEntity.Location,
            Visibility = eventEntity.Visibility,
            CreatedBy = creator?.Username ?? "Unknown",
            CreatedAt = eventEntity.CreatedAt,
            IsActive = eventEntity.IsActive
        };
    }

    public async Task<EventResponse?> UpdateAsync(int id, EventUpdateRequest request, int userId)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(id);
        if (eventEntity == null) return null;

        // Solo el creador o un admin/astrónomo puede actualizar
        if (eventEntity.CreatedBy != userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || (user.Role != UserRole.Admin && user.Role != UserRole.Astronomer))
                throw new ForbiddenException("You are not allowed to update this event");
        }

        if (!string.IsNullOrEmpty(request.Title)) eventEntity.Title = request.Title;
        if (request.Description != null) eventEntity.Description = request.Description;
        if (request.EventType.HasValue) eventEntity.EventType = request.EventType.Value;
        if (request.StartDate.HasValue) eventEntity.StartDate = request.StartDate.Value;
        if (request.EndDate.HasValue) eventEntity.EndDate = request.EndDate.Value;
        if (request.Location != null) eventEntity.Location = request.Location;
        if (request.Visibility != null) eventEntity.Visibility = request.Visibility;
        if (request.IsActive.HasValue) eventEntity.IsActive = request.IsActive.Value;

        await _eventRepository.UpdateAsync(eventEntity);

        var creator = await _userRepository.GetByIdAsync(eventEntity.CreatedBy);

        return new EventResponse
        {
            EventId = eventEntity.EventId,
            Title = eventEntity.Title,
            Description = eventEntity.Description,
            EventType = eventEntity.EventType.ToString(),
            StartDate = eventEntity.StartDate,
            EndDate = eventEntity.EndDate,
            Location = eventEntity.Location,
            Visibility = eventEntity.Visibility,
            CreatedBy = creator?.Username ?? "Unknown",
            CreatedAt = eventEntity.CreatedAt,
            IsActive = eventEntity.IsActive
        };
    }

    public async Task<bool> DeactivateAsync(int eventId, int userId)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(eventId);
        if (eventEntity == null) throw new NotFoundException("Event", eventId);

        // Solo el creador o un admin/astrónomo puede desactivar
        if (eventEntity.CreatedBy != userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || (user.Role != UserRole.Admin && user.Role != UserRole.Astronomer))
                throw new ForbiddenException("You are not allowed to deactivate this event");
        }

        return await _eventRepository.DeactivateAsync(eventId);
    }
}