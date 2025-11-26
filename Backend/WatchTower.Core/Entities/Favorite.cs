namespace WatchTower.Core.Entities;

public class Favorite
{
    public int FavoriteId { get; set; }
    public int UserId { get; set; }
    public int? CelestialBodyId { get; set; }
    public int? ArticleId { get; set; }
    public int? DiscoveryId { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual CelestialBody? CelestialBody { get; set; }
    public virtual Article? Article { get; set; }
    public virtual Discovery? Discovery { get; set; }
}