namespace Observatorio.Mvc.Models.Admin;

public class SystemInfoViewModel
{
    public DateTime ServerTime { get; set; }
    public string ServerName { get; set; }
    public string OSVersion { get; set; }
    public int ProcessorCount { get; set; }
    public TimeSpan SystemUpTime { get; set; }
    public long MemoryUsage { get; set; }
    public int TotalUsers { get; set; }
    public int TotalLogs { get; set; }
    public int TotalDiscoveries { get; set; }
    public int TotalArticles { get; set; }
    public int TotalEvents { get; set; }
}