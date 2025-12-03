namespace Observatorio.Mvc.Models.Article;

public class ArticleIndexViewModel
{
    public List<ArticleViewModel> Articles { get; set; } = new();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}