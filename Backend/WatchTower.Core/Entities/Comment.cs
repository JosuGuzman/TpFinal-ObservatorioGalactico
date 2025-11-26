namespace WatchTower.Core.Entities;

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