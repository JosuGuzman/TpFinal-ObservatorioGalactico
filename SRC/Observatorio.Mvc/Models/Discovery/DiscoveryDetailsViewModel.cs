namespace Observatorio.Mvc.Models.Discovery;

public class DiscoveryDetailsViewModel
{
    public DiscoveryViewModel Discovery { get; set; }
    public List<DiscoveryVoteViewModel> Votes { get; set; } = new();
    public bool HasUserVoted { get; set; }
    public bool? UserVote { get; set; }
}