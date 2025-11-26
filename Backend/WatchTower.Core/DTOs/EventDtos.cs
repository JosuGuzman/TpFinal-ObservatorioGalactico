// DTOs/EventDtos.cs
namespace WatchTower.Core.DTOs;

public class EventSearchRequest
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public EventType? EventType { get; set; }
    public bool ActiveOnly { get; set; } = true;
}

public class EventCreateRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public EventType EventType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Location { get; set; }
    public string? Visibility { get; set; }
}

public class EventUpdateRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public EventType? EventType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Location { get; set; }
    public string? Visibility { get; set; }
    public bool? IsActive { get; set; }
}

public class EventResponse
{
    public int EventId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string EventType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Location { get; set; }
    public string? Visibility { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}