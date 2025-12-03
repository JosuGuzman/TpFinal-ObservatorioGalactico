namespace Observatorio.Core.Entities.Astronomic;

public class Planet : CelestialBody
{
    public int PlanetID { get; set; }
    public int StarID { get; set; }
    public PlanetType PlanetType { get; set; }
    public double? MassEarth { get; set; }
    public double? RadiusEarth { get; set; }
    public double? OrbitalPeriodDays { get; set; }
    public double? OrbitalDistanceAU { get; set; }
    public double? Eccentricity { get; set; }
    public double? HabitabilityScore { get; set; }
    public string Atmosphere { get; set; } // JSON
    public double? SurfaceTempK { get; set; }
    public DateTime? DiscoveryDate { get; set; }
    
    public virtual Star Star { get; set; }
    
    public bool IsPotentiallyHabitable
    {
        get => HabitabilityScore.HasValue && HabitabilityScore.Value >= 0.7;
    }
    
    public double? Gravity
    {
        get
        {
            if (MassEarth.HasValue && RadiusEarth.HasValue && RadiusEarth > 0)
            {
                return MassEarth.Value / (RadiusEarth.Value * RadiusEarth.Value);
            }
            return null;
        }
    }
}