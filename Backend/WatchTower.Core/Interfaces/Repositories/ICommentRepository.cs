namespace WatchTower.Core.Interfaces.Repositories;

public interface ICommentRepository
{
    Task<Comment?> GetByIdAsync(int commentId);
    Task<IEnumerable<Comment>> GetByDiscoveryAsync(int discoveryId);
    Task<IEnumerable<Comment>> GetByArticleAsync(int articleId);
    Task<int> CreateAsync(Comment comment);
    Task UpdateAsync(Comment comment);
    Task<bool> DeactivateAsync(int commentId);
}