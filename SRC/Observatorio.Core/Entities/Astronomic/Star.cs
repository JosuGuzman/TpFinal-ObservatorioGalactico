namespace Observatorio.Core.Entities.Astronomic;

public class Star : CelestialBody
{
    public int StarID { get; set; }
    public int? GalaxyID { get; set; }
    public SpectralType SpectralType { get; set; }
    public int? SurfaceTempK { get; set; }
    public double? MassSolar { get; set; }
    public double? RadiusSolar { get; set; }
    public double? LuminositySolar { get; set; }
    public double? DistanceLy { get; set; }
    public double? RadialVelocity { get; set; }
    public float? ApparentMagnitude { get; set; }
    public double? EstimatedAgeMillionYears { get; set; }
    
    public virtual Galaxy Galaxy { get; set; }
    public virtual ICollection<Planet> Planets { get; set; } = new List<Planet>();
    
    public string Color
    {
        get
        {
            return SpectralType switch
            {
                SpectralType.O => "Blue",
                SpectralType.B => "Blue-White",
                SpectralType.A => "White",
                SpectralType.F => "Yellow-White",
                SpectralType.G => "Yellow",
                SpectralType.K => "Orange",
                SpectralType.M => "Red",
                _ => "Unknown"
            };
        }
    }
    
    public double? AbsoluteMagnitude
    {
        get
        {
            if (ApparentMagnitude.HasValue && DistanceLy.HasValue && DistanceLy > 0)
            {
                return ApparentMagnitude.Value - 5 * (Math.Log10(DistanceLy.Value * 0.306601) - 1);
            }
            return null;
        }
    }
}