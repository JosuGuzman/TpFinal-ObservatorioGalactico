namespace WatchTower.Infrastructure.Data.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CommentRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Comment?> GetByIdAsync(int commentId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT c.*, u.Username as UserName
            FROM Comments c
            INNER JOIN Users u ON c.UserId = u.UserId
            WHERE c.CommentId = @CommentId";
        
        return await connection.QueryFirstOrDefaultAsync<Comment>(sql, new { CommentId = commentId });
    }

    public async Task<IEnumerable<Comment>> GetByDiscoveryAsync(int discoveryId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT c.*, u.Username as UserName
            FROM Comments c
            INNER JOIN Users u ON c.UserId = u.UserId
            WHERE c.DiscoveryId = @DiscoveryId AND c.IsActive = 1
            ORDER BY c.CreatedAt ASC";
        
        return await connection.QueryAsync<Comment>(sql, new { DiscoveryId = discoveryId });
    }

    public async Task<IEnumerable<Comment>> GetByArticleAsync(int articleId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT c.*, u.Username as UserName
            FROM Comments c
            INNER JOIN Users u ON c.UserId = u.UserId
            WHERE c.ArticleId = @ArticleId AND c.IsActive = 1
            ORDER BY c.CreatedAt ASC";
        
        return await connection.QueryAsync<Comment>(sql, new { ArticleId = articleId });
    }

    public async Task<int> CreateAsync(Comment comment)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO Comments (Content, UserId, DiscoveryId, ArticleId, ParentCommentId)
            VALUES (@Content, @UserId, @DiscoveryId, @ArticleId, @ParentCommentId);
            SELECT LAST_INSERT_ID();";
        
        return await connection.ExecuteScalarAsync<int>(sql, comment);
    }

    public async Task UpdateAsync(Comment comment)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE Comments SET 
            Content = @Content, IsActive = @IsActive
            WHERE CommentId = @CommentId";
        
        await connection.ExecuteAsync(sql, comment);
    }

    public async Task<bool> DeactivateAsync(int commentId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "UPDATE Comments SET IsActive = 0 WHERE CommentId = @CommentId";
        var affected = await connection.ExecuteAsync(sql, new { CommentId = commentId });
        return affected > 0;
    }
}