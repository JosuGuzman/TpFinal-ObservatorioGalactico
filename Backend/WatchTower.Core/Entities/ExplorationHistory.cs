namespace WatchTower.Core.Entities;

public class ExplorationHistory
{
    public int HistoryId { get; set; }
    public int UserId { get; set; }
    public int CelestialBodyId { get; set; }
    public DateTime VisitedAt { get; set; }
    public int? TimeSpent { get; set; } // in seconds
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual CelestialBody CelestialBody { get; set; } = null!;
}