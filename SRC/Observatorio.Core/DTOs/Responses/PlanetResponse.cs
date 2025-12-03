namespace Observatorio.Core.DTOs.Responses;

public class PlanetResponse
{
    public int PlanetID { get; set; }
    public int StarID { get; set; }
    public string StarName { get; set; }
    public string Name { get; set; }
    public string PlanetType { get; set; }
    public double? MassEarth { get; set; }
    public double? RadiusEarth { get; set; }
    public double? Gravity { get; set; }
    public double? OrbitalPeriodDays { get; set; }
    public double? OrbitalDistanceAU { get; set; }
    public double? Eccentricity { get; set; }
    public double? HabitabilityScore { get; set; }
    public bool IsPotentiallyHabitable { get; set; }
    public double? SurfaceTempK { get; set; }
    public DateTime? DiscoveryDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}