namespace WatchTower.Infrastructure.Data.Repositories;

public class ArticleRepository : IArticleRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ArticleRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Article?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT a.*, u.Username as AuthorName 
            FROM Articles a 
            INNER JOIN Users u ON a.AuthorId = u.UserId 
            WHERE a.ArticleId = @Id";
        
        return await connection.QueryFirstOrDefaultAsync<Article>(sql, new { Id = id });
    }

    public async Task<IEnumerable<Article>> SearchAsync(ArticleSearchRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var parameters = new
        {
            search_term = request.SearchTerm,
            category_filter = request.Category,
            published_only = request.PublishedOnly
        };

        return await connection.QueryAsync<Article>(
            "CALL SearchArticles(@search_term, @category_filter, @published_only)",
            parameters);
    }

    public async Task<int> CreateAsync(Article article)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO Articles (Title, Content, Summary, AuthorId, Category, IsPublished)
            VALUES (@Title, @Content, @Summary, @AuthorId, @Category, @IsPublished);
            SELECT LAST_INSERT_ID();";
        
        return await connection.ExecuteScalarAsync<int>(sql, article);
    }

    public async Task UpdateAsync(Article article)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE Articles SET 
            Title = @Title, Content = @Content, Summary = @Summary, 
            Category = @Category, IsPublished = @IsPublished, PublishedAt = @PublishedAt
            WHERE ArticleId = @ArticleId";
        
        await connection.ExecuteAsync(sql, article);
    }

    public async Task<bool> PublishAsync(int articleId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "UPDATE Articles SET IsPublished = 1, PublishedAt = NOW() WHERE ArticleId = @ArticleId";
        var affected = await connection.ExecuteAsync(sql, new { ArticleId = articleId });
        return affected > 0;
    }

    public async Task<bool> UnpublishAsync(int articleId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "UPDATE Articles SET IsPublished = 0, PublishedAt = NULL WHERE ArticleId = @ArticleId";
        var affected = await connection.ExecuteAsync(sql, new { ArticleId = articleId });
        return affected > 0;
    }

    public async Task IncrementViewCountAsync(int articleId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "UPDATE Articles SET ViewCount = ViewCount + 1 WHERE ArticleId = @ArticleId";
        await connection.ExecuteAsync(sql, new { ArticleId = articleId });
    }

    public async Task<IEnumerable<Article>> GetRecentArticlesAsync(int count)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM Articles WHERE IsPublished = 1 ORDER BY PublishedAt DESC LIMIT @Count";
        return await connection.QueryAsync<Article>(sql, new { Count = count });
    }

    public async Task<bool> IsAuthorAsync(int articleId, int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT COUNT(1) FROM Articles WHERE ArticleId = @ArticleId AND AuthorId = @UserId";
        var count = await connection.ExecuteScalarAsync<int>(sql, new { ArticleId = articleId, UserId = userId });
        return count > 0;
    }

    public async Task<IEnumerable<Article>> GetArticlesByCategoryAsync(ArticleCategory category)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM Articles WHERE Category = @Category AND IsPublished = 1 ORDER BY PublishedAt DESC";
        return await connection.QueryAsync<Article>(sql, new { Category = category.ToString() });
    }
}