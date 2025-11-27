namespace WatchTower.Infrastructure.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IUserRepository _userRepository;

    public CommentService(ICommentRepository commentRepository, IUserRepository userRepository)
    {
        _commentRepository = commentRepository;
        _userRepository = userRepository;
    }

    public async Task<CommentResponse?> GetByIdAsync(int id)
    {
        var comment = await _commentRepository.GetByIdAsync(id);
        if (comment == null) return null;

        return new CommentResponse
        {
            CommentId = comment.CommentId,
            Content = comment.Content,
            UserName = comment.UserId.ToString(), // Se reemplazaría con nombre real
            UserId = comment.UserId,
            CreatedAt = comment.CreatedAt,
            IsActive = comment.IsActive,
            Replies = new List<CommentResponse>() // Se cargarían las respuestas
        };
    }

    public async Task<IEnumerable<CommentResponse>> GetByDiscoveryAsync(int discoveryId)
    {
        var comments = await _commentRepository.GetByDiscoveryAsync(discoveryId);
        return await BuildCommentTree(comments);
    }

    public async Task<IEnumerable<CommentResponse>> GetByArticleAsync(int articleId)
    {
        var comments = await _commentRepository.GetByArticleAsync(articleId);
        return await BuildCommentTree(comments);
    }

    public async Task<CommentResponse> CreateAsync(CommentCreateRequest request, int userId)
    {
        var comment = new Comment
        {
            Content = request.Content,
            UserId = userId,
            DiscoveryId = request.DiscoveryId,
            ArticleId = request.ArticleId,
            ParentCommentId = request.ParentCommentId,
            IsActive = true
        };

        var commentId = await _commentRepository.CreateAsync(comment);
        comment.CommentId = commentId;

        var user = await _userRepository.GetByIdAsync(userId);

        return new CommentResponse
        {
            CommentId = comment.CommentId,
            Content = comment.Content,
            UserName = user?.Username ?? "Unknown",
            UserId = comment.UserId,
            CreatedAt = comment.CreatedAt,
            IsActive = comment.IsActive,
            Replies = new List<CommentResponse>()
        };
    }

    public async Task<CommentResponse?> UpdateAsync(int id, CommentUpdateRequest request, int userId)
    {
        var comment = await _commentRepository.GetByIdAsync(id);
        if (comment == null) return null;

        // Solo el usuario que creó el comentario puede actualizarlo
        if (comment.UserId != userId)
            throw new ForbiddenException("You are not allowed to update this comment");

        comment.Content = request.Content;

        await _commentRepository.UpdateAsync(comment);

        var user = await _userRepository.GetByIdAsync(comment.UserId);

        return new CommentResponse
        {
            CommentId = comment.CommentId,
            Content = comment.Content,
            UserName = user?.Username ?? "Unknown",
            UserId = comment.UserId,
            CreatedAt = comment.CreatedAt,
            IsActive = comment.IsActive,
            Replies = new List<CommentResponse>()
        };
    }

    public async Task<bool> DeactivateAsync(int commentId, int userId)
    {
        var comment = await _commentRepository.GetByIdAsync(commentId);
        if (comment == null) throw new NotFoundException("Comment", commentId);

        // Solo el usuario que creó el comentario o un admin/astrónomo puede desactivarlo
        if (comment.UserId != userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || (user.Role != Core.Enums.UserRole.Admin && user.Role != Core.Enums.UserRole.Astronomer))
                throw new ForbiddenException("You are not allowed to deactivate this comment");
        }

        return await _commentRepository.DeactivateAsync(commentId);
    }

    private async Task<IEnumerable<CommentResponse>> BuildCommentTree(IEnumerable<Comment> comments)
    {
        var commentDict = new Dictionary<int, CommentResponse>();
        var rootComments = new List<CommentResponse>();

        // Primera pasada: crear todos los CommentResponse
        foreach (var comment in comments.Where(c => c.IsActive))
        {
            var user = await _userRepository.GetByIdAsync(comment.UserId);
            
            var commentResponse = new CommentResponse
            {
                CommentId = comment.CommentId,
                Content = comment.Content,
                UserName = user?.Username ?? "Unknown",
                UserId = comment.UserId,
                CreatedAt = comment.CreatedAt,
                IsActive = comment.IsActive,
                Replies = new List<CommentResponse>()
            };

            commentDict[comment.CommentId] = commentResponse;

            if (comment.ParentCommentId == null)
            {
                rootComments.Add(commentResponse);
            }
        }

        // Segunda pasada: construir el árbol
        foreach (var comment in comments.Where(c => c.ParentCommentId.HasValue && c.IsActive))
        {
            if (commentDict.TryGetValue(comment.ParentCommentId.Value, out var parentComment))
            {
                var childResponse = commentDict[comment.CommentId];
                parentComment.Replies = parentComment.Replies.Append(childResponse);
            }
        }

        return rootComments;
    }
}