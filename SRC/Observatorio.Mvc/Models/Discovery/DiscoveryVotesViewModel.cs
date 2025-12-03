namespace Observatorio.Mvc.Models.Discovery;

public class DiscoveryVotesViewModel
{
    public int DiscoveryID { get; set; }
    public List<DiscoveryVoteViewModel> Votes { get; set; } = new();
    public int Upvotes { get; set; }
    public int Downvotes { get; set; }
    public int TotalVotes { get; set; }
    public double ApprovalRate { get; set; }
}