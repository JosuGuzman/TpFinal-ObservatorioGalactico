namespace Observatorio.Core.Services;

public class ContentService : IContentService
{
    private readonly IArticleRepository _articleRepository;
    private readonly IEventRepository _eventRepository;
    private readonly ILoggingService _loggingService;

    public ContentService(
        IArticleRepository articleRepository,
        IEventRepository eventRepository,
        ILoggingService loggingService)
    {
        _articleRepository = articleRepository;
        _eventRepository = eventRepository;
        _loggingService = loggingService;
    }

    // Artículos
    public async Task<Article> CreateArticleAsync(string title, string content, int authorId, 
        string tags = null, string featuredImage = null)
    {
        try
        {
            var article = new Article
            {
                Title = title,
                Slug = StringHelpers.GenerateSlug(title),
                Content = content,
                AuthorUserID = authorId,
                Tags = tags,
                FeaturedImage = featuredImage,
                State = ArticleState.Borrador,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdArticle = await _articleRepository.AddAsync(article);
            
            await _loggingService.LogInfoAsync("ArticleCreated", 
                $"Article created: {title} by author {authorId}", authorId);

            return createdArticle;
        }
        catch (Exception ex)
        {
            await _loggingService.LogErrorAsync("ArticleCreation", 
                $"Error creating article: {title}", authorId, null, ex);
            throw;
        }
    }

    public async Task<Article> GetArticleByIdAsync(int id)
    {
        var article = await _articleRepository.GetByIdAsync(id);
        if (article == null)
            throw new NotFoundException("Article", id);

        return article;
    }

    public async Task<Article> GetArticleBySlugAsync(string slug)
    {
        var articles = await _articleRepository.SearchByTitleAsync(slug);
        var article = articles.FirstOrDefault(a => a.Slug == slug);
        
        if (article == null)
            throw new NotFoundException("Article", slug);

        return article;
    }

    public async Task<IEnumerable<Article>> GetPublishedArticlesAsync()
    {
        return await _articleRepository.GetPublishedAsync();
    }

    public async Task<IEnumerable<Article>> GetArticlesByAuthorAsync(int authorId)
    {
        return await _articleRepository.GetByAuthorAsync(authorId);
    }

    public async Task<Article> UpdateArticleAsync(int articleId, string title = null, 
        string content = null, string state = null, string tags = null)
    {
        var article = await GetArticleByIdAsync(articleId);

        if (!string.IsNullOrEmpty(title))
        {
            article.Title = title;
            article.Slug = StringHelpers.GenerateSlug(title);
        }

        if (!string.IsNullOrEmpty(content))
            article.Content = content;

        if (!string.IsNullOrEmpty(state))
            article.State = Enum.Parse<ArticleState>(state);

        if (tags != null)
            article.Tags = tags;

        article.UpdatedAt = DateTime.UtcNow;

        await _articleRepository.UpdateAsync(article);
        
        await _loggingService.LogInfoAsync("ArticleUpdated", 
            $"Article updated: {article.Title} (ID: {articleId})", article.AuthorUserID);

        return article;
    }

    public async Task DeleteArticleAsync(int articleId)
    {
        var article = await GetArticleByIdAsync(articleId);
        await _articleRepository.DeleteAsync(articleId);
        
        await _loggingService.LogInfoAsync("ArticleDeleted", 
            $"Article deleted: {article.Title} (ID: {articleId})", article.AuthorUserID);
    }

    public async Task PublishArticleAsync(int articleId)
    {
        var article = await GetArticleByIdAsync(articleId);
        article.State = ArticleState.Publicado;
        article.PublishedAt = DateTime.UtcNow;
        article.UpdatedAt = DateTime.UtcNow;

        await _articleRepository.UpdateAsync(article);
        
        await _loggingService.LogInfoAsync("ArticlePublished", 
            $"Article published: {article.Title} (ID: {articleId})", article.AuthorUserID);
    }

    public async Task UnpublishArticleAsync(int articleId)
    {
        var article = await GetArticleByIdAsync(articleId);
        article.State = ArticleState.Borrador;
        article.UpdatedAt = DateTime.UtcNow;

        await _articleRepository.UpdateAsync(article);
        
        await _loggingService.LogInfoAsync("ArticleUnpublished", 
            $"Article unpublished: {article.Title} (ID: {articleId})", article.AuthorUserID);
    }

    // Eventos
    public async Task<Event> CreateEventAsync(string name, string type, DateTime eventDate, 
        string description, int createdBy, string visibility = null, string coordinates = null, 
        int? durationMinutes = null, string resources = null)
    {
        try
        {
            var eventObj = new Event
            {
                Name = name,
                Type = Enum.Parse<EventType>(type),
                EventDate = eventDate,
                Description = description,
                Visibility = visibility,
                Coordinates = coordinates,
                DurationMinutes = durationMinutes,
                Resources = resources,
                CreatedByUserID = createdBy,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdEvent = await _eventRepository.AddAsync(eventObj);
            
            await _loggingService.LogInfoAsync("EventCreated", 
                $"Event created: {name} by user {createdBy}", createdBy);

            return createdEvent;
        }
        catch (Exception ex)
        {
            await _loggingService.LogErrorAsync("EventCreation", 
                $"Error creating event: {name}", createdBy, null, ex);
            throw;
        }
    }

    public async Task<Event> GetEventByIdAsync(int id)
    {
        var eventObj = await _eventRepository.GetByIdAsync(id);
        if (eventObj == null)
            throw new NotFoundException("Event", id);

        return eventObj;
    }

    public async Task<IEnumerable<Event>> GetUpcomingEventsAsync(int limit = 10)
    {
        return await _eventRepository.GetUpcomingAsync(DateTime.UtcNow);
    }

    public async Task<IEnumerable<Event>> GetEventsByTypeAsync(string type)
    {
        return await _eventRepository.GetByTypeAsync(type);
    }

    public async Task<IEnumerable<Event>> GetRecentEventsAsync(int limit = 10)
    {
        return await _eventRepository.GetRecentAsync(limit);
    }

    public async Task<Event> UpdateEventAsync(int eventId, string name = null, string type = null, 
        DateTime? eventDate = null, string description = null)
    {
        var eventObj = await GetEventByIdAsync(eventId);

        if (!string.IsNullOrEmpty(name))
            eventObj.Name = name;

        if (!string.IsNullOrEmpty(type))
            eventObj.Type = Enum.Parse<EventType>(type);

        if (eventDate.HasValue)
            eventObj.EventDate = eventDate.Value;

        if (!string.IsNullOrEmpty(description))
            eventObj.Description = description;

        eventObj.UpdatedAt = DateTime.UtcNow;

        await _eventRepository.UpdateAsync(eventObj);
        
        await _loggingService.LogInfoAsync("EventUpdated", 
            $"Event updated: {eventObj.Name} (ID: {eventId})", eventObj.CreatedByUserID);

        return eventObj;
    }

    public async Task DeleteEventAsync(int eventId)
    {
        var eventObj = await GetEventByIdAsync(eventId);
        await _eventRepository.DeleteAsync(eventId);
        
        await _loggingService.LogInfoAsync("EventDeleted", 
            $"Event deleted: {eventObj.Name} (ID: {eventId})", eventObj.CreatedByUserID);
    }
}