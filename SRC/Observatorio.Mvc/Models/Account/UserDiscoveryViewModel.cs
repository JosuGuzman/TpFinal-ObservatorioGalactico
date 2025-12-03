namespace Observatorio.Mvc.Models.Account;

public class UserDiscoveryViewModel
{
    public int DiscoveryID { get; set; }
    public string ObjectType { get; set; }
    public string SuggestedName { get; set; }
    public string State { get; set; }
    public DateTime CreatedAt { get; set; }
    public int Upvotes { get; set; }
    public int Downvotes { get; set; }
    public double ApprovalRate { get; set; }
}