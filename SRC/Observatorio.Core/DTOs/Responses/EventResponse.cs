namespace Observatorio.Core.DTOs.Responses;

public class EventResponse
{
    public int EventID { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public DateTime EventDate { get; set; }
    public string Description { get; set; }
    public List<string> Visibility { get; set; }
    public int? DurationMinutes { get; set; }
    public int CreatedByUserID { get; set; }
    public string CreatedByName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsUpcoming { get; set; }
    public string TimeUntilEvent { get; set; }
}