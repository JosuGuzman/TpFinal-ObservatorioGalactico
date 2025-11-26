// DTOs/ArticleDtos.cs
namespace WatchTower.Core.DTOs;

public class ArticleSearchRequest
{
    public string? SearchTerm { get; set; }
    public string? Category { get; set; }
    public bool PublishedOnly { get; set; } = true;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; }
}

public class ArticleCreateRequest
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public ArticleCategory Category { get; set; }
}

public class ArticleUpdateRequest
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? Summary { get; set; }
    public ArticleCategory? Category { get; set; }
    public bool? IsPublished { get; set; }
}

public class ArticleResponse
{
    public int ArticleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string Category { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int ViewCount { get; set; }
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
}

public class ArticleDetailResponse : ArticleResponse
{
    public string Content { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public IEnumerable<CommentResponse> Comments { get; set; } = new List<CommentResponse>();
}