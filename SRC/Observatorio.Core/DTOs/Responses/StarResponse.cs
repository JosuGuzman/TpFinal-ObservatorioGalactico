namespace Observatorio.Core.DTOs.Responses;

public class StarResponse
{
    public int StarID { get; set; }
    public int? GalaxyID { get; set; }
    public string GalaxyName { get; set; }
    public string Name { get; set; }
    public string SpectralType { get; set; }
    public string Color { get; set; }
    public int? SurfaceTempK { get; set; }
    public double? MassSolar { get; set; }
    public double? RadiusSolar { get; set; }
    public double? LuminositySolar { get; set; }
    public double? DistanceLy { get; set; }
    public double RA { get; set; }
    public double Dec { get; set; }
    public double? RadialVelocity { get; set; }
    public float? ApparentMagnitude { get; set; }
    public double? AbsoluteMagnitude { get; set; }
    public double? EstimatedAgeMillionYears { get; set; }
    public int PlanetCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}