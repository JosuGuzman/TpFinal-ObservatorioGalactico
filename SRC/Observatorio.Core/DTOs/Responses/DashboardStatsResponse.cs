namespace Observatorio.Core.DTOs.Responses;

public class DashboardStatsResponse
{
    public int TotalGalaxies { get; set; }
    public int TotalStars { get; set; }
    public int TotalPlanets { get; set; }
    public int TotalUsers { get; set; }
    public int TotalDiscoveries { get; set; }
    public int TotalArticles { get; set; }
    public int TotalEvents { get; set; }
    public int PendingDiscoveries { get; set; }
    public int UpcomingEvents { get; set; }
    public int HabitablePlanets { get; set; }
}