namespace Observatorio.Mvc.Models.Admin;

public class SystemLogsViewModel
{
    public List<SystemLogViewModel> Logs { get; set; } = new();
    public string EventType { get; set; }
    public string Status { get; set; }
    public int? UserId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Search { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public string SortBy { get; set; }
    public bool SortDescending { get; set; }
    public List<string> AvailableEventTypes { get; set; } = new();
    public List<string> AvailableStatuses { get; set; } = new();
    public LogStatsViewModel Stats { get; set; }
}