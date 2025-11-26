namespace WatchTower.Core.Interfaces.Services;

public interface IArticleService
{
    Task<ArticleDetailResponse?> GetByIdAsync(int id);
    Task<PagedResult<ArticleResponse>> SearchAsync(ArticleSearchRequest request);
    Task<ArticleResponse> CreateAsync(ArticleCreateRequest request, int authorId);
    Task<ArticleResponse?> UpdateAsync(int id, ArticleUpdateRequest request, int userId);
    Task<bool> PublishAsync(int articleId, int userId);
    Task<bool> UnpublishAsync(int articleId, int userId);
    Task<bool> IncrementViewCountAsync(int id);
}