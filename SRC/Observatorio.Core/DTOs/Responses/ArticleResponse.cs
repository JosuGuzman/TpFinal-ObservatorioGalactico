namespace Observatorio.Core.DTOs.Responses;

public class ArticleResponse
{
    public int ArticleID { get; set; }
    public string Title { get; set; }
    public string Slug { get; set; }
    public string Content { get; set; }
    public int AuthorUserID { get; set; }
    public string AuthorName { get; set; }
    public string State { get; set; }
    public List<string> Tags { get; set; }
    public string FeaturedImage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsPublished { get; set; }
}