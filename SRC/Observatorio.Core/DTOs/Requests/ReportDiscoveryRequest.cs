namespace Observatorio.Core.DTOs.Requests;

public class ReportDiscoveryRequest
{
    [Required]
    public string ObjectType { get; set; }

    [StringLength(250)]
    public string SuggestedName { get; set; }

    [Required, Range(0, 360)]
    public double RA { get; set; }

    [Required, Range(-90, 90)]
    public double Dec { get; set; }

    [Required, StringLength(5000)]
    public string Description { get; set; }

    public string Attachments { get; set; }
}