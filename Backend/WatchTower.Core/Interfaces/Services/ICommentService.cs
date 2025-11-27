namespace WatchTower.Core.Interfaces.Services;

public interface ICommentService
{
    Task<CommentResponse?> GetByIdAsync(int id);
    Task<IEnumerable<CommentResponse>> GetByDiscoveryAsync(int discoveryId);
    Task<IEnumerable<CommentResponse>> GetByArticleAsync(int articleId);
    Task<CommentResponse> CreateAsync(CommentCreateRequest request, int userId);
    Task<CommentResponse?> UpdateAsync(int id, CommentUpdateRequest request, int userId);
    Task<bool> DeactivateAsync(int commentId, int userId);
}