namespace Observatorio.Mvc.Models.Account;

public class UserArticleViewModel
{
    public int ArticleID { get; set; }
    public string Title { get; set; }
    public string State { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public bool IsPublished { get; set; }
}