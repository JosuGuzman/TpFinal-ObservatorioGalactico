namespace Observatorio.Mvc.Models.Event;

public class CalendarEventViewModel
{
    public int EventID { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public DateTime EventDate { get; set; }
    public string Description { get; set; }
    public bool IsUpcoming { get; set; }
}