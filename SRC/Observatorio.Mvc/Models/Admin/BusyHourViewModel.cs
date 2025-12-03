namespace Observatorio.Mvc.Models.Admin;

public class BusyHourViewModel
{
    public int Hour { get; set; }
    public int TotalLogs { get; set; }
    public double AvgLogsPerDay { get; set; }
}