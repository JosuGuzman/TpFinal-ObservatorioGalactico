namespace Observatorio.Core.Entities
{
    public class Article
    {
        public int ArticleId { get; set; }
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public string Summary { get; set; } = "";
        public int AuthorId { get; set; }
        public string Category { get; set; } = "";
        public bool IsPublished { get; set; }
        public DateTime? PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int ViewCount { get; set; }
        
        // Navigation
        public required User Author { get; set; }
    }
}