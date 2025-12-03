namespace Observatorio.Mvc.Models.Article;

public class ArticleViewModel
{
    public int ArticleID { get; set; }
    public string Title { get; set; }
    public string Slug { get; set; }
    public string Content { get; set; }
    public string AuthorName { get; set; }
    public int AuthorUserID { get; set; }
    public string State { get; set; }
    public string Tags { get; set; }
    public string FeaturedImage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsPublished { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    public int Views { get; set; }
}