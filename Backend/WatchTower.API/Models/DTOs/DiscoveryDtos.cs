namespace WatchTower.API.Models.DTOs;

public class DiscoveryCreateRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Coordinates { get; set; }
    public int? CelestialBodyId { get; set; }
}

public class DiscoveryResponse
{
    public int DiscoveryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string ReportedBy { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? CelestialBodyName { get; set; }
}