namespace Observatorio.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
public class ArticlesController : BaseApiController
{
    private readonly IContentService _contentService;
    private readonly ILoggingService _loggingService;

    public ArticlesController(IContentService contentService, ILoggingService loggingService)
    {
        _contentService = contentService;
        _loggingService = loggingService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublishedArticles([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var articles = await _contentService.GetPublishedArticlesAsync();
            
            var pagedArticles = articles
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return SuccessResponse(new
            {
                data = pagedArticles,
                page,
                pageSize,
                totalCount = articles.Count(),
                totalPages = (int)Math.Ceiling((double)articles.Count() / pageSize)
            });
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving articles", ex);
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetArticleById(int id)
    {
        try
        {
            var article = await _contentService.GetArticleByIdAsync(id);
            
            if (article == null)
                return NotFoundResponse($"Article with ID {id} not found");

            // Solo artículos publicados o el autor/admin pueden ver borradores
            if (!article.IsPublished && article.AuthorUserID != GetCurrentUserId() && !IsAdmin())
                return ForbiddenResponse("You don't have permission to view this article");

            return SuccessResponse(article);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving article", ex);
        }
    }

    [HttpGet("slug/{slug}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetArticleBySlug(string slug)
    {
        try
        {
            var article = await _contentService.GetArticleBySlugAsync(slug);
            
            if (article == null)
                return NotFoundResponse($"Article with slug '{slug}' not found");

            if (!article.IsPublished && article.AuthorUserID != GetCurrentUserId() && !IsAdmin())
                return ForbiddenResponse("You don't have permission to view this article");

            return SuccessResponse(article);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving article", ex);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Researcher,Astronomer,Admin")]
    public async Task<IActionResult> CreateArticle([FromBody] CreateArticleRequest request)
    {
        try
        {
            var authorId = GetCurrentUserId();
            
            var article = await _contentService.CreateArticleAsync(
                request.Title,
                request.Content,
                authorId,
                request.Tags,
                request.FeaturedImage);

            await _loggingService.LogInfoAsync("ArticleCreated", 
                $"Article '{request.Title}' created by user {authorId}", 
                authorId);

            return CreatedResponse($"/api/v1/articles/{article.ArticleID}", article);
        }
        catch (Exception ex)
        {
            return BadRequestResponse(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Researcher,Astronomer,Admin")]
    public async Task<IActionResult> UpdateArticle(int id, [FromBody] UpdateArticleRequest request)
    {
        try
        {
            var article = await _contentService.GetArticleByIdAsync(id);
            
            if (article == null)
                return NotFoundResponse($"Article with ID {id} not found");

            // Solo el autor o un administrador puede actualizar
            if (article.AuthorUserID != GetCurrentUserId() && !IsAdmin())
                return ForbiddenResponse("You can only update your own articles");

            var updatedArticle = await _contentService.UpdateArticleAsync(
                id, 
                request.Title, 
                request.Content, 
                request.State, 
                request.Tags);

            return SuccessResponse(updatedArticle, "Article updated successfully");
        }
        catch (Exception ex)
        {
            return BadRequestResponse(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Researcher,Astronomer,Admin")]
    public async Task<IActionResult> DeleteArticle(int id)
    {
        try
        {
            var article = await _contentService.GetArticleByIdAsync(id);
            
            if (article == null)
                return NotFoundResponse($"Article with ID {id} not found");

            // Solo el autor o un administrador puede eliminar
            if (article.AuthorUserID != GetCurrentUserId() && !IsAdmin())
                return ForbiddenResponse("You can only delete your own articles");

            await _contentService.DeleteArticleAsync(id);
            
            await _loggingService.LogInfoAsync("ArticleDeleted", 
                $"Article {id} deleted", 
                GetCurrentUserId());

            return SuccessResponse(new { message = $"Article {id} deleted successfully" });
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error deleting article", ex);
        }
    }

    [HttpPost("{id}/publish")]
    [Authorize(Roles = "Astronomer,Admin")]
    public async Task<IActionResult> PublishArticle(int id)
    {
        try
        {
            await _contentService.PublishArticleAsync(id);
            
            await _loggingService.LogInfoAsync("ArticlePublished", 
                $"Article {id} published", 
                GetCurrentUserId());

            return SuccessResponse(new { message = "Article published successfully" });
        }
        catch (Exception ex)
        {
            return BadRequestResponse(ex.Message);
        }
    }

    [HttpPost("{id}/unpublish")]
    [Authorize(Roles = "Astronomer,Admin")]
    public async Task<IActionResult> UnpublishArticle(int id)
    {
        try
        {
            await _contentService.UnpublishArticleAsync(id);
            
            return SuccessResponse(new { message = "Article unpublished successfully" });
        }
        catch (Exception ex)
        {
            return BadRequestResponse(ex.Message);
        }
    }

    [HttpGet("author/{authorId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetArticlesByAuthor(int authorId)
    {
        try
        {
            var articles = await _contentService.GetArticlesByAuthorAsync(authorId);
            var publishedArticles = articles.Where(a => a.IsPublished);
            
            // Solo el autor o un administrador puede ver los borradores
            if (authorId != GetCurrentUserId() && !IsAdmin())
                return SuccessResponse(publishedArticles);
            
            return SuccessResponse(articles);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving author articles", ex);
        }
    }

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> SearchArticles([FromQuery] string query, [FromQuery] int limit = 20)
    {
        try
        {
            var articles = await _contentService.SearchArticlesAsync(query);
            var publishedArticles = articles.Where(a => a.IsPublished).Take(limit);
            
            return SuccessResponse(publishedArticles);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error searching articles", ex);
        }
    }

    [HttpGet("latest")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLatestArticles([FromQuery] int limit = 5)
    {
        try
        {
            var articles = await _contentService.GetLatestArticlesAsync(limit);
            return SuccessResponse(articles);
        }
        catch (Exception ex)
        {
            return InternalErrorResponse("Error retrieving latest articles", ex);
        }
    }

    // Clases auxiliares y extensiones
    public class UpdateArticleRequest
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? State { get; set; }
        public string? Tags { get; set; }
    }
}

// Extensiones para IContentService
public static class ContentServiceExtensions
{
    public static Task<IEnumerable<Article>> SearchArticlesAsync(this IContentService service, string query)
    {
        // Implementación simple
        return Task.FromResult(Enumerable.Empty<Article>());
    }

    public static Task<IEnumerable<Article>> GetLatestArticlesAsync(this IContentService service, int limit)
    {
        // Implementación simple
        return Task.FromResult(Enumerable.Empty<Article>());
    }
}