namespace Observatorio.Mvc.Models.Event;

public class EventSearchViewModel
{
    public List<EventViewModel> Events { get; set; } = new();
    public string Query { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}