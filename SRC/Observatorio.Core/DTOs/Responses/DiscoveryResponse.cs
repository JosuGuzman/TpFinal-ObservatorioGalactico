namespace Observatorio.Core.DTOs.Responses;

public class DiscoveryResponse
{
    public int DiscoveryID { get; set; }
    public int ReporterUserID { get; set; }
    public string ReporterName { get; set; }
    public string ObjectType { get; set; }
    public string SuggestedName { get; set; }
    public double RA { get; set; }
    public double Dec { get; set; }
    public string Description { get; set; }
    public string State { get; set; }
    public int Upvotes { get; set; }
    public int Downvotes { get; set; }
    public int TotalVotes { get; set; }
    public double ApprovalRate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}