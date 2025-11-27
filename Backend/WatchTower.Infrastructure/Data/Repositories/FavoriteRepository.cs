namespace WatchTower.Infrastructure.Data.Repositories;

public class FavoriteRepository : IFavoriteRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public FavoriteRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Favorite?> GetByIdAsync(int favoriteId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM Favorites WHERE FavoriteId = @FavoriteId";
        return await connection.QueryFirstOrDefaultAsync<Favorite>(sql, new { FavoriteId = favoriteId });
    }

    public async Task<IEnumerable<Favorite>> GetUserFavoritesAsync(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT f.*, 
                   cb.Name as CelestialBodyName,
                   a.Title as ArticleTitle,
                   d.Title as DiscoveryTitle
            FROM Favorites f
            LEFT JOIN CelestialBodies cb ON f.CelestialBodyId = cb.BodyId
            LEFT JOIN Articles a ON f.ArticleId = a.ArticleId
            LEFT JOIN Discoveries d ON f.DiscoveryId = d.DiscoveryId
            WHERE f.UserId = @UserId
            ORDER BY f.CreatedAt DESC";

        return await connection.QueryAsync<Favorite>(sql, new { UserId = userId });
    }

    public async Task<bool> AddFavoriteAsync(Favorite favorite)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO Favorites (UserId, CelestialBodyId, ArticleId, DiscoveryId)
            VALUES (@UserId, @CelestialBodyId, @ArticleId, @DiscoveryId)";

        try
        {
            var affected = await connection.ExecuteAsync(sql, favorite);
            return affected > 0;
        }
        catch (Exception)
        {
            return false; // Unique constraint violation
        }
    }

    public async Task<bool> RemoveFavoriteAsync(int favoriteId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "DELETE FROM Favorites WHERE FavoriteId = @FavoriteId";
        var affected = await connection.ExecuteAsync(sql, new { FavoriteId = favoriteId });
        return affected > 0;
    }

    public async Task<bool> IsFavoriteAsync(int userId, int? celestialBodyId, int? articleId, int? discoveryId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT COUNT(1) FROM Favorites 
            WHERE UserId = @UserId 
            AND CelestialBodyId = @CelestialBodyId 
            AND ArticleId = @ArticleId 
            AND DiscoveryId = @DiscoveryId";

        var count = await connection.ExecuteScalarAsync<int>(sql, new
        {
            UserId = userId,
            CelestialBodyId = celestialBodyId,
            ArticleId = articleId,
            DiscoveryId = discoveryId
        });

        return count > 0;
    }

    public async Task<bool> RemoveFavoriteByItemAsync(int userId, int? celestialBodyId, int? articleId, int? discoveryId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            DELETE FROM Favorites 
            WHERE UserId = @UserId 
            AND CelestialBodyId = @CelestialBodyId 
            AND ArticleId = @ArticleId 
            AND DiscoveryId = @DiscoveryId";

        var affected = await connection.ExecuteAsync(sql, new
        {
            UserId = userId,
            CelestialBodyId = celestialBodyId,
            ArticleId = articleId,
            DiscoveryId = discoveryId
        });

        return affected > 0;
    }
}