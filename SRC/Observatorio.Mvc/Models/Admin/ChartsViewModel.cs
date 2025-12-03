namespace Observatorio.Mvc.Models.Admin;

public class ChartsViewModel
{
    public string Period { get; set; }
    public List<ChartDataViewModel> UserRegistrations { get; set; } = new();
    public List<ChartDataViewModel> DiscoveryReports { get; set; } = new();
    public List<ChartDataViewModel> ArticlePublications { get; set; } = new();
    public List<ChartDataViewModel> EventCreations { get; set; } = new();
    public List<ChartDataViewModel> UserActivity { get; set; } = new();
    public List<ChartDataViewModel> SystemUsage { get; set; } = new();
}