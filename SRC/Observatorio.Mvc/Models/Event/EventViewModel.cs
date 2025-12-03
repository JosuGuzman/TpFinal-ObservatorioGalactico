namespace Observatorio.Mvc.Models.Event;

public class EventViewModel
{
    public int EventID { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public DateTime EventDate { get; set; }
    public string Description { get; set; }
    public string Visibility { get; set; }
    public string Coordinates { get; set; }
    public int? DurationMinutes { get; set; }
    public string Resources { get; set; }
    public int CreatedByUserID { get; set; }
    public string CreatedByName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsUpcoming { get; set; }
    public string TimeUntilEvent { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
}