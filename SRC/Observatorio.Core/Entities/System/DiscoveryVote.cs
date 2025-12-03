namespace Observatorio.Core.Entities.System;

public class DiscoveryVote
{
    public int VoteID { get; set; }
    public int DiscoveryID { get; set; }
    public int VoterUserID { get; set; }
    public bool Vote { get; set; } // true = upvote, false = downvote
    public string Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public virtual Content.Discovery Discovery { get; set; }
    public virtual User.User Voter { get; set; }
}