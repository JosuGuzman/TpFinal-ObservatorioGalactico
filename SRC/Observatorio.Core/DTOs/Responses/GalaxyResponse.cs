namespace Observatorio.Core.DTOs.Responses;

public class GalaxyResponse
{
    public int GalaxyID { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public double DistanceLy { get; set; }
    public float? ApparentMagnitude { get; set; }
    public double? AbsoluteMagnitude { get; set; }
    public double RA { get; set; }
    public double Dec { get; set; }
    public double? Redshift { get; set; }
    public string Description { get; set; }
    public string Discoverer { get; set; }
    public int? YearDiscovery { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}