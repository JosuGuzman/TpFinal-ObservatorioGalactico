namespace Observatorio.Core.Interfaces;

public interface IArticleRepository : IRepository<Article>
{
    Task<IEnumerable<Article>> GetByAuthorAsync(int authorId);
    Task<IEnumerable<Article>> GetPublishedAsync();
    Task<IEnumerable<Article>> GetByStateAsync(string state);
    Task<IEnumerable<Article>> SearchByTitleAsync(string title);
    Task<IEnumerable<Article>> GetLatestPublishedAsync(int limit);
    Task<int> CountByStateAsync(string state);
}