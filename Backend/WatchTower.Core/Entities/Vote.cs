namespace WatchTower.Core.Entities;

public class Vote
{
    public int VoteId { get; set; }
    public int DiscoveryId { get; set; }
    public int UserId { get; set; }
    public VoteType VoteType { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public virtual Discovery Discovery { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}