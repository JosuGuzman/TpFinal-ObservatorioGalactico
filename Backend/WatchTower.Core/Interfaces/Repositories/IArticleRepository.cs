namespace WatchTower.Core.Interfaces.Repositories;

public interface IArticleRepository
{
    Task<Article?> GetByIdAsync(int id);
    Task<IEnumerable<Article>> SearchAsync(ArticleSearchRequest request);
    Task<int> CreateAsync(Article article);
    Task UpdateAsync(Article article);
    Task<bool> PublishAsync(int articleId);
    Task<bool> UnpublishAsync(int articleId);
    Task IncrementViewCountAsync(int articleId);
    Task<IEnumerable<Article>> GetRecentArticlesAsync(int count);
    Task<bool> IsAuthorAsync(int articleId, int userId);
}