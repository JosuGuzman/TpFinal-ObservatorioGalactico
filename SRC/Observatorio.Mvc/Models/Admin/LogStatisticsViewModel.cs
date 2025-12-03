namespace Observatorio.Mvc.Models.Admin;

public class LogStatisticsViewModel
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalLogs { get; set; }
    public List<LogTypeStatViewModel> LogsByType { get; set; } = new();
    public List<LogStatusStatViewModel> LogsByStatus { get; set; } = new();
    public List<LogUserStatViewModel> LogsByUser { get; set; } = new();
    public List<HourlyStatViewModel> LogsByHour { get; set; } = new();
    public List<DailyStatViewModel> LogsByDay { get; set; } = new();
    public List<ErrorTrendViewModel> ErrorTrend { get; set; } = new();
    public List<BusyHourViewModel> BusiestHours { get; set; } = new();
}