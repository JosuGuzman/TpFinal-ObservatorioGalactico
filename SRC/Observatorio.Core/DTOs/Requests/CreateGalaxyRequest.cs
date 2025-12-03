namespace Observatorio.Core.DTOs.Requests;

public class CreateGalaxyRequest
{
    [Required, StringLength(200)]
    public string Name { get; set; }

    [Required]
    public string Type { get; set; }

    [Required, Range(0, double.MaxValue)]
    public double DistanceLy { get; set; }

    public float? ApparentMagnitude { get; set; }

    [Required, Range(0, 360)]
    public double RA { get; set; }

    [Required, Range(-90, 90)]
    public double Dec { get; set; }

    public double? Redshift { get; set; }

    public string Description { get; set; }

    public string ImageURLs { get; set; }

    public string Discoverer { get; set; }

    [Range(1000, 9999)]
    public int? YearDiscovery { get; set; }

    public string References { get; set; }
}