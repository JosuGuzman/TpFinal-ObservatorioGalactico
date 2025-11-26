namespace WatchTower.Core.Entities;

public class Discovery
{
    public int DiscoveryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Coordinates { get; set; }
    public DateTime? DiscoveryDate { get; set; }
    public int ReportedBy { get; set; }
    public int? CelestialBodyId { get; set; }
    public DiscoveryStatus Status { get; set; } = DiscoveryStatus.Pending;
    public string? NASA_API_Data { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public int? VerifiedBy { get; set; }
    
    // Navigation properties
    public virtual User Reporter { get; set; } = null!;
    public virtual CelestialBody? CelestialBody { get; set; }
    public virtual User? Verifier { get; set; }
    public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    
    // Calculated properties
    public int Rating => Votes?.Count(v => v.VoteType == VoteType.Up) - Votes?.Count(v => v.VoteType == VoteType.Down) ?? 0;
}
