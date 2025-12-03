namespace Observatorio.Core.Entities.Astronomic;

public class Galaxy : CelestialBody
{
    public int GalaxyID { get; set; }
    public GalaxyType Type { get; set; }
    public double DistanceLy { get; set; }
    public float? ApparentMagnitude { get; set; }
    public double? Redshift { get; set; }
    public string ImageURLs { get; set; } // JSON
    public string Discoverer { get; set; }
    public int? YearDiscovery { get; set; }
    public string References { get; set; } // JSON

    public virtual ICollection<Star> Stars { get; set; } = new List<Star>();

    public double? AbsoluteMagnitude
    {
        get
        {
            if (ApparentMagnitude.HasValue && DistanceLy > 0)
            {
                return ApparentMagnitude.Value - 5 * (Math.Log10(DistanceLy * 0.306601) - 1);
            }
            return null;
        }
    }
}