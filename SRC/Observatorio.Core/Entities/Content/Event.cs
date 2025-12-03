namespace Observatorio.Core.Entities.Content;

public class Event
{
    public int EventID { get; set; }
    public string Name { get; set; }
    public EventType Type { get; set; }
    public DateTime EventDate { get; set; }
    public string Description { get; set; }
    public string Visibility { get; set; } // JSON
    public string Coordinates { get; set; } // JSON
    public int? DurationMinutes { get; set; }
    public string Resources { get; set; } // JSON
    public int CreatedByUserID { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public virtual User.User CreatedBy { get; set; }
    
    public bool IsUpcoming => EventDate > DateTime.UtcNow;
    public bool IsPast => EventDate <= DateTime.UtcNow;
    public TimeSpan? TimeUntilEvent => IsUpcoming ? EventDate - DateTime.UtcNow : null;
}