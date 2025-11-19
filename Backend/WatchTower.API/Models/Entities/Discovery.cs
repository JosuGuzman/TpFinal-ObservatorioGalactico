namespace WatchTower.API.Models.Entities;

public class Discovery
{
    public int DiscoveryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Coordinates { get; set; }
    public DateTime? DiscoveryDate { get; set; }
    public int ReportedBy { get; set; }
    public int? CelestialBodyId { get; set; }
    public string Status { get; set; } = "Pending";
    public string? NASA_API_Data { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public int? VerifiedBy { get; set; }
}
