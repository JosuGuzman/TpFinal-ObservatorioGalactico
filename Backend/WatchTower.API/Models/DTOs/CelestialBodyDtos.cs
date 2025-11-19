namespace WatchTower.API.Models.DTOs;

public class CelestialBodySearchRequest
{
    public string? SearchTerm { get; set; }
    public string? BodyType { get; set; }
    public decimal? MaxDistance { get; set; }
    public decimal? MinMagnitude { get; set; }
}

public class CelestialBodyResponse
{
    public int BodyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? SubType { get; set; }
    public string? Constellation { get; set; }
    public decimal? Distance { get; set; }
    public decimal? ApparentMagnitude { get; set; }
    public string? Description { get; set; }
    public string? NASA_ImageURL { get; set; }
}