namespace WatchTower.Infrastructure.Data.Repositories;

public class ExplorationRepository : IExplorationRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ExplorationRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<ExplorationHistory>> GetUserHistoryAsync(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT eh.*, cb.Name as CelestialBodyName
            FROM ExplorationHistory eh
            INNER JOIN CelestialBodies cb ON eh.CelestialBodyId = cb.BodyId
            WHERE eh.UserId = @UserId
            ORDER BY eh.VisitedAt DESC";
        
        return await connection.QueryAsync<ExplorationHistory>(sql, new { UserId = userId });
    }

    public async Task<bool> AddToHistoryAsync(ExplorationHistory history)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO ExplorationHistory (UserId, CelestialBodyId, VisitedAt, TimeSpent)
            VALUES (@UserId, @CelestialBodyId, @VisitedAt, @TimeSpent)
            ON DUPLICATE KEY UPDATE TimeSpent = TimeSpent + @TimeSpent, VisitedAt = @VisitedAt";
        
        var affected = await connection.ExecuteAsync(sql, history);
        return affected > 0;
    }

    public async Task<bool> UpdateTimeSpentAsync(int historyId, int timeSpent)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "UPDATE ExplorationHistory SET TimeSpent = TimeSpent + @TimeSpent WHERE HistoryId = @HistoryId";
        var affected = await connection.ExecuteAsync(sql, new { HistoryId = historyId, TimeSpent = timeSpent });
        return affected > 0;
    }

    public async Task<int> GetTotalExplorationTimeAsync(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT COALESCE(SUM(TimeSpent), 0) FROM ExplorationHistory WHERE UserId = @UserId";
        return await connection.ExecuteScalarAsync<int>(sql, new { UserId = userId });
    }

    public async Task<int> GetUniqueBodiesExploredCountAsync(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT COUNT(DISTINCT CelestialBodyId) FROM ExplorationHistory WHERE UserId = @UserId";
        return await connection.ExecuteScalarAsync<int>(sql, new { UserId = userId });
    }
}