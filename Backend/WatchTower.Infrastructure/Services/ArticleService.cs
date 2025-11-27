namespace WatchTower.Infrastructure.Services;

public class ArticleService : IArticleService
{
    private readonly IArticleRepository _articleRepository;
    private readonly IUserRepository _userRepository;

    public ArticleService(IArticleRepository articleRepository, IUserRepository userRepository)
    {
        _articleRepository = articleRepository;
        _userRepository = userRepository;
    }

    public async Task<ArticleDetailResponse?> GetByIdAsync(int id)
    {
        var article = await _articleRepository.GetByIdAsync(id);
        if (article == null) return null;

        await _articleRepository.IncrementViewCountAsync(id);

        return new ArticleDetailResponse
        {
            ArticleId = article.ArticleId,
            Title = article.Title,
            Content = article.Content,
            Summary = article.Summary,
            Category = article.Category.ToString(),
            AuthorName = article.AuthorId.ToString(), // Se reemplazaría con el nombre real del autor
            CreatedAt = article.CreatedAt,
            ViewCount = article.ViewCount + 1, // Incrementado
            IsPublished = article.IsPublished,
            PublishedAt = article.PublishedAt,
            UpdatedAt = article.UpdatedAt,
            Comments = new List<CommentResponse>() // Se cargarían desde el repositorio de comentarios
        };
    }

    public async Task<PagedResult<ArticleResponse>> SearchAsync(ArticleSearchRequest request)
    {
        var articles = await _articleRepository.SearchAsync(request);
        
        // En una implementación real, esto vendría de una consulta COUNT
        var totalCount = articles.Count();

        var response = articles.Select(article => new ArticleResponse
        {
            ArticleId = article.ArticleId,
            Title = article.Title,
            Summary = article.Summary,
            Category = article.Category.ToString(),
            AuthorName = article.AuthorId.ToString(),
            CreatedAt = article.CreatedAt,
            ViewCount = article.ViewCount,
            IsPublished = article.IsPublished,
            PublishedAt = article.PublishedAt
        });

        return new PagedResult<ArticleResponse>
        {
            Items = response,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public async Task<ArticleResponse> CreateAsync(ArticleCreateRequest request, int authorId)
    {
        var article = new Article
        {
            Title = request.Title,
            Content = request.Content,
            Summary = request.Summary,
            Category = request.Category,
            AuthorId = authorId,
            IsPublished = false
        };

        var articleId = await _articleRepository.CreateAsync(article);
        article.ArticleId = articleId;

        var author = await _userRepository.GetByIdAsync(authorId);

        return new ArticleResponse
        {
            ArticleId = article.ArticleId,
            Title = article.Title,
            Summary = article.Summary,
            Category = article.Category.ToString(),
            AuthorName = author?.Username ?? "Unknown",
            CreatedAt = article.CreatedAt,
            ViewCount = 0,
            IsPublished = article.IsPublished,
            PublishedAt = article.PublishedAt
        };
    }

    public async Task<ArticleResponse?> UpdateAsync(int id, ArticleUpdateRequest request, int userId)
    {
        var article = await _articleRepository.GetByIdAsync(id);
        if (article == null) return null;

        // Verificar permisos
        if (article.AuthorId != userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || (user.Role != UserRole.Admin && user.Role != UserRole.Astronomer))
                throw new ForbiddenException("You are not allowed to update this article");
        }

        if (!string.IsNullOrEmpty(request.Title)) article.Title = request.Title;
        if (!string.IsNullOrEmpty(request.Content)) article.Content = request.Content;
        if (request.Summary != null) article.Summary = request.Summary;
        if (request.Category.HasValue) article.Category = request.Category.Value;
        if (request.IsPublished.HasValue) article.IsPublished = request.IsPublished.Value;

        await _articleRepository.UpdateAsync(article);

        var author = await _userRepository.GetByIdAsync(article.AuthorId);

        return new ArticleResponse
        {
            ArticleId = article.ArticleId,
            Title = article.Title,
            Summary = article.Summary,
            Category = article.Category.ToString(),
            AuthorName = author?.Username ?? "Unknown",
            CreatedAt = article.CreatedAt,
            ViewCount = article.ViewCount,
            IsPublished = article.IsPublished,
            PublishedAt = article.PublishedAt
        };
    }

    public async Task<bool> PublishAsync(int articleId, int userId)
    {
        var article = await _articleRepository.GetByIdAsync(articleId);
        if (article == null) throw new NotFoundException("Article", articleId);

        // Verificar permisos
        if (article.AuthorId != userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || (user.Role != UserRole.Admin && user.Role != UserRole.Astronomer))
                throw new ForbiddenException("You are not allowed to publish this article");
        }

        return await _articleRepository.PublishAsync(articleId);
    }

    public async Task<bool> UnpublishAsync(int articleId, int userId)
    {
        var article = await _articleRepository.GetByIdAsync(articleId);
        if (article == null) throw new NotFoundException("Article", articleId);

        // Verificar permisos
        if (article.AuthorId != userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || (user.Role != UserRole.Admin && user.Role != UserRole.Astronomer))
                throw new ForbiddenException("You are not allowed to unpublish this article");
        }

        return await _articleRepository.UnpublishAsync(articleId);
    }

    public async Task<bool> IncrementViewCountAsync(int id)
    {
        var article = await _articleRepository.GetByIdAsync(id);
        if (article == null) return false;

        await _articleRepository.IncrementViewCountAsync(id);
        return true;
    }

    public async Task<IEnumerable<ArticleResponse>> GetRecentArticlesAsync(int count)
    {
        var articles = await _articleRepository.GetRecentArticlesAsync(count);
        return articles.Select(article => new ArticleResponse
        {
            ArticleId = article.ArticleId,
            Title = article.Title,
            Summary = article.Summary,
            Category = article.Category.ToString(),
            AuthorName = article.AuthorId.ToString(),
            CreatedAt = article.CreatedAt,
            ViewCount = article.ViewCount,
            IsPublished = article.IsPublished,
            PublishedAt = article.PublishedAt
        });
    }
}