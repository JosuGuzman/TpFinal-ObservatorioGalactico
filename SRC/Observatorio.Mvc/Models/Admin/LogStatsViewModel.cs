namespace Observatorio.Mvc.Models.Admin;

public class LogStatsViewModel
{
    public int TotalLogs { get; set; }
    public int InfoLogs { get; set; }
    public int WarningLogs { get; set; }
    public int ErrorLogs { get; set; }
    public int SecurityLogs { get; set; }
    public int UniqueUsers { get; set; }
    public int UniqueIPs { get; set; }
}