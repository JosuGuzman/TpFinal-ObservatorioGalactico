using Observatorio.Core.Models;
using Observatorio.Core.Entities;

namespace Observatorio.Core.Interfaces
{
    public interface IRepoArticle
    {
        Task<Article> GetByIdAsync(int id);
        Task<IEnumerable<Article>> GetAllAsync();
        Task<IEnumerable<Article>> SearchAsync(SearchRequest request);
        Task<int> CreateAsync(Article article);
        Task UpdateAsync(Article article);
        Task DeleteAsync(int id);
        Task IncrementViewCountAsync(int articleId);
    }
}