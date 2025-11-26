namespace WatchTower.Core.Entities;

public class CelestialBody
{
    public int BodyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public CelestialBodyType Type { get; set; }
    public string? SubType { get; set; }
    public string? Constellation { get; set; }
    public string? RightAscension { get; set; }
    public string? Declination { get; set; }
    public decimal? Distance { get; set; }
    public decimal? ApparentMagnitude { get; set; }
    public decimal? AbsoluteMagnitude { get; set; }
    public decimal? Mass { get; set; }
    public decimal? Radius { get; set; }
    public int? Temperature { get; set; }
    public string? Description { get; set; }
    public DateTime? DiscoveryDate { get; set; }
    public string? NASA_ImageURL { get; set; }
    public bool IsVerified { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public virtual User? Creator { get; set; }
    public virtual ICollection<Discovery> Discoveries { get; set; } = new List<Discovery>();
}