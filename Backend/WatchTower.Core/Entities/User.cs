// Entities/User.cs
namespace WatchTower.Core.Entities;

public class User
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Visitor;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
    
    // Navigation properties
    public virtual ICollection<Discovery> Discoveries { get; set; } = new List<Discovery>();
    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
}

// Entities/CelestialBody.cs
public class CelestialBody
{
    public int BodyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public CelestialBodyType Type { get; set; }
    public string? SubType { get; set; }
    public string? Constellation { get; set; }
    public string? RightAscension { get; set; }
    public string? Declination { get; set; }
    public decimal? Distance { get; set; }
    public decimal? ApparentMagnitude { get; set; }
    public decimal? AbsoluteMagnitude { get; set; }
    public decimal? Mass { get; set; }
    public decimal? Radius { get; set; }
    public int? Temperature { get; set; }
    public string? Description { get; set; }
    public DateTime? DiscoveryDate { get; set; }
    public string? NASA_ImageURL { get; set; }
    public bool IsVerified { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public virtual User? Creator { get; set; }
    public virtual ICollection<Discovery> Discoveries { get; set; } = new List<Discovery>();
}

// Entities/Discovery.cs
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

// Entities/Article.cs
public class Article
{
    public int ArticleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public int AuthorId { get; set; }
    public ArticleCategory Category { get; set; }
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int ViewCount { get; set; }
    
    // Navigation properties
    public virtual User Author { get; set; } = null!;
}

// Entities/Event.cs
public class Event
{
    public int EventId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public EventType EventType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Location { get; set; }
    public string? Visibility { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual User Creator { get; set; } = null!;
}

// Entities/Favorite.cs
public class Favorite
{
    public int FavoriteId { get; set; }
    public int UserId { get; set; }
    public int? CelestialBodyId { get; set; }
    public int? ArticleId { get; set; }
    public int? DiscoveryId { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual CelestialBody? CelestialBody { get; set; }
    public virtual Article? Article { get; set; }
    public virtual Discovery? Discovery { get; set; }
}

// Entities/Comment.cs
public class Comment
{
    public int CommentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int UserId { get; set; }
    public int? DiscoveryId { get; set; }
    public int? ArticleId { get; set; }
    public int? ParentCommentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Discovery? Discovery { get; set; }
    public virtual Article? Article { get; set; }
    public virtual Comment? ParentComment { get; set; }
    public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();
}

// Entities/Vote.cs
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

// Entities/ExplorationHistory.cs
public class ExplorationHistory
{
    public int HistoryId { get; set; }
    public int UserId { get; set; }
    public int CelestialBodyId { get; set; }
    public DateTime VisitedAt { get; set; }
    public int? TimeSpent { get; set; } // in seconds
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual CelestialBody CelestialBody { get; set; } = null!;
}