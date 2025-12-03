namespace Observatorio.Mvc.Models.Article;

public class ArticleByAuthorViewModel
{
    public List<ArticleViewModel> Articles { get; set; } = new();
    public int AuthorId { get; set; }
    public string AuthorName { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}