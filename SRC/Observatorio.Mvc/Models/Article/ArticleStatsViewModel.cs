namespace Observatorio.Mvc.Models.Article;

public class ArticleStatsViewModel
{
    public int TotalArticles { get; set; }
    public int PublishedArticles { get; set; }
    public int DraftArticles { get; set; }
    public int ArchivedArticles { get; set; }
    public int TotalViews { get; set; }
    public int AverageViewsPerArticle { get; set; }
    public List<ArticleTrendViewModel> Trends { get; set; } = new();
}

public class ArticleTrendViewModel
{
    public string Period { get; set; }
    public int Count { get; set; }
    public int Views { get; set; }
}