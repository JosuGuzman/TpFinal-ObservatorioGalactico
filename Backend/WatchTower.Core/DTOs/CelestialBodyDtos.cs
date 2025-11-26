namespace WatchTower.Core.DTOs;

public class CelestialBodySearchRequest
{
    public string? SearchTerm { get; set; }
    public string? BodyType { get; set; }
    public decimal? MaxDistance { get; set; }
    public decimal? MinMagnitude { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; }
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
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CelestialBodyDetailResponse : CelestialBodyResponse
{
    public string? RightAscension { get; set; }
    public string? Declination { get; set; }
    public decimal? AbsoluteMagnitude { get; set; }
    public decimal? Mass { get; set; }
    public decimal? Radius { get; set; }
    public int? Temperature { get; set; }
    public DateTime? DiscoveryDate { get; set; }
    public string? CreatedBy { get; set; }
}
