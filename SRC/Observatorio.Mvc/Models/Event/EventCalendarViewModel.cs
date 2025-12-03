namespace Observatorio.Mvc.Models.Event;

public class EventCalendarViewModel
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; }
    public List<CalendarEventViewModel> Events { get; set; } = new();
}