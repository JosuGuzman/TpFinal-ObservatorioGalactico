namespace Observatorio.Mvc.Models.Event;

public class EventIndexViewModel
{
    public List<EventViewModel> Events { get; set; } = new();
    public List<string> EventTypes { get; set; } = new();
    public string SelectedType { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}