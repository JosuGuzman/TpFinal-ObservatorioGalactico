namespace Observatorio.Core.Interfaces;

public interface IContentService
{
    Task<Article> CreateArticleAsync(string title, string content, int authorId, 
                                    string tags = null, string featuredImage = null);
    Task<Article> GetArticleByIdAsync(int id);
    Task<Article> GetArticleBySlugAsync(string slug);
    Task<IEnumerable<Event>> GetAllEventsAsync();
    Task<IEnumerable<Article>> GetPublishedArticlesAsync();
    Task<IEnumerable<Article>> GetArticlesByAuthorAsync(int authorId);
    Task<Article> UpdateArticleAsync(int articleId, string? title = null, string? content = null, 
                                    string? state = null, string? tags = null);
    Task DeleteArticleAsync(int articleId);
    Task PublishArticleAsync(int articleId);
    Task UnpublishArticleAsync(int articleId);
    
    Task<Event> CreateEventAsync(string name, string type, DateTime eventDate, 
                                string description, int createdBy, string visibility = null, 
                                string coordinates = null, int? durationMinutes = null, 
                                string resources = null);
    Task<Event> GetEventByIdAsync(int id);
    Task<IEnumerable<Event>> GetUpcomingEventsAsync(int limit = 10);
    Task<IEnumerable<Event>> GetEventsByTypeAsync(string type);
    Task<IEnumerable<Event>> GetRecentEventsAsync(int limit = 10);
    Task<Event> UpdateEventAsync(int eventId, string name = null, string type = null, 
                                DateTime? eventDate = null, string description = null);
    Task DeleteEventAsync(int eventId);
}