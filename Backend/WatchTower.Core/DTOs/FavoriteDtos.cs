namespace WatchTower.Core.DTOs;

public class FavoriteCreateRequest
{
    public int? CelestialBodyId { get; set; }
    public int? ArticleId { get; set; }
    public int? DiscoveryId { get; set; }
}

public class FavoriteResponse
{
    public int FavoriteId { get; set; }
    public string ItemType { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public int? ItemId { get; set; }
    public DateTime CreatedAt { get; set; }
}