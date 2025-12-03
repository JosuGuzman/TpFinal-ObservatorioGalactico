namespace Observatorio.Mvc.Models.Discovery;

public class DiscoveryVoteViewModel
{
    public int VoteID { get; set; }
    public int VoterUserID { get; set; }
    public string VoterName { get; set; }
    public bool Vote { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}