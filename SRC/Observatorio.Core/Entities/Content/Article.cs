namespace Observatorio.Core.Entities.Content;

public class Article
{
    public int ArticleID { get; set; }
    public string Title { get; set; }
    public string Slug { get; set; }
    public string Content { get; set; }
    public int AuthorUserID { get; set; }
    public ArticleState State { get; set; } = ArticleState.Draft;
    public string Tags { get; set; } // JSON
    public string FeaturedImage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PublishedAt { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public virtual User.User Author { get; set; }
    
    public bool IsPublished => State == ArticleState.Published && PublishedAt.HasValue;
    public bool CanView => IsPublished || State == ArticleState.Draft;
}