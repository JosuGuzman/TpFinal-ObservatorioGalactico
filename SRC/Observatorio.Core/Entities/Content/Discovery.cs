namespace Observatorio.Core.Entities.Content;

public class Discovery
{
    public int DiscoveryID { get; set; }
    public int ReporterUserID { get; set; }
    public ObjectType ObjectType { get; set; }
    public string SuggestedName { get; set; }
    public double RA { get; set; }
    public double Dec { get; set; }
    public string Description { get; set; }
    public string Attachments { get; set; } // JSON
    public DiscoveryState State { get; set; } = DiscoveryState.Pendiente;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public virtual User.User Reporter { get; set; }
    public virtual ICollection<DiscoveryVote> Votes { get; set; } = new List<DiscoveryVote>();
    
    public int Upvotes => Votes?.Count(v => v.Vote) ?? 0;
    public int Downvotes => Votes?.Count(v => !v.Vote) ?? 0;
    public int TotalVotes => Upvotes + Downvotes;
    public double ApprovalRate => TotalVotes > 0 ? (double)Upvotes / TotalVotes : 0;
    public bool IsApproved => State == DiscoveryState.Aprobado;
    public bool IsPending => State == DiscoveryState.Pendiente;
    public bool InCommunityValidation => State == DiscoveryState.ValidacionComunitaria;
    public bool UnderAstronomerReview => State == DiscoveryState.RevisadoAstronomo;
}