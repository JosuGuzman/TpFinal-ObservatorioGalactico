namespace Observatorio.Mvc.Models.Admin;

public class DashboardViewModel
{
    public int TotalGalaxies { get; set; }
    public int TotalStars { get; set; }
    public int TotalPlanets { get; set; }
    public int TotalUsers { get; set; }
    public int TotalDiscoveries { get; set; }
    public int TotalArticles { get; set; }
    public int TotalEvents { get; set; }
    public int PendingDiscoveries { get; set; }
    public int ActiveUsers { get; set; }
    public int HabitablePlanets { get; set; }
    public int NewUsersToday { get; set; }
    public int NewDiscoveriesToday { get; set; }
}