namespace WatchTower.Infrastructure.Data.Repositories;

public class DiscoveryRepository : IDiscoveryRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DiscoveryRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Discovery?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT d.*, u.Username as ReporterName, v.Username as VerifierName, cb.Name as CelestialBodyName
            FROM Discoveries d
            LEFT JOIN Users u ON d.ReportedBy = u.UserId
            LEFT JOIN Users v ON d.VerifiedBy = v.UserId
            LEFT JOIN CelestialBodies cb ON d.CelestialBodyId = cb.BodyId
            WHERE d.DiscoveryId = @Id";
        
        return await connection.QueryFirstOrDefaultAsync<Discovery>(sql, new { Id = id });
    }

    public async Task<IEnumerable<Discovery>> SearchAsync(DiscoverySearchRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var parameters = new
        {
            search_term = request.SearchTerm,
            status_filter = request.Status,
            min_rating = request.MinRating
        };

        return await connection.QueryAsync<Discovery>(
            "CALL SearchDiscoveries(@search_term, @status_filter, @min_rating)",
            parameters);
    }

    public async Task<int> CreateAsync(Discovery discovery)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var parameters = new
        {
            p_title = discovery.Title,
            p_description = discovery.Description,
            p_coordinates = discovery.Coordinates,
            p_reported_by = discovery.ReportedBy,
            p_celestial_body_id = discovery.CelestialBodyId
        };

        var result = await connection.QuerySingleAsync<dynamic>(
            "CALL AddDiscovery(@p_title, @p_description, @p_coordinates, @p_reported_by, @p_celestial_body_id)",
            parameters);
        
        return result.NewDiscoveryId;
    }

    public async Task UpdateAsync(Discovery discovery)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE Discoveries SET 
            Title = @Title, Description = @Description, Coordinates = @Coordinates,
            CelestialBodyId = @CelestialBodyId, Status = @Status, NASA_API_Data = @NASA_API_Data,
            VerifiedAt = @VerifiedAt, VerifiedBy = @VerifiedBy
            WHERE DiscoveryId = @DiscoveryId";
        
        await connection.ExecuteAsync(sql, discovery);
    }

    public async Task<bool> AddVoteAsync(int discoveryId, int userId, VoteType voteType)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        try
        {
            const string sql = @"
                INSERT INTO Votes (DiscoveryId, UserId, VoteType) 
                VALUES (@DiscoveryId, @UserId, @VoteType)
                ON DUPLICATE KEY UPDATE VoteType = @VoteType";
            
            var affected = await connection.ExecuteAsync(sql, new 
            { 
                DiscoveryId = discoveryId, 
                UserId = userId, 
                VoteType = voteType.ToString() 
            });
            
            return affected > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> UpdateStatusAsync(int discoveryId, DiscoveryStatus status, int verifiedBy)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE Discoveries 
            SET Status = @Status, VerifiedAt = NOW(), VerifiedBy = @VerifiedBy 
            WHERE DiscoveryId = @DiscoveryId";
        
        var affected = await connection.ExecuteAsync(sql, new 
        { 
            DiscoveryId = discoveryId, 
            Status = status.ToString(), 
            VerifiedBy = verifiedBy 
        });
        
        return affected > 0;
    }

    public async Task<IEnumerable<Discovery>> GetUserDiscoveriesAsync(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM Discoveries WHERE ReportedBy = @UserId ORDER BY CreatedAt DESC";
        return await connection.QueryAsync<Discovery>(sql, new { UserId = userId });
    }

    public async Task<int> GetDiscoveryCountAsync(DiscoveryStatus? status = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT COUNT(*) FROM Discoveries";
        if (status.HasValue)
        {
            sql += " WHERE Status = @Status";
            return await connection.ExecuteScalarAsync<int>(sql, new { Status = status.ToString() });
        }
        return await connection.ExecuteScalarAsync<int>(sql);
    }

    public async Task<bool> HasUserVotedAsync(int discoveryId, int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT COUNT(1) FROM Votes WHERE DiscoveryId = @DiscoveryId AND UserId = @UserId";
        var count = await connection.ExecuteScalarAsync<int>(sql, new { DiscoveryId = discoveryId, UserId = userId });
        return count > 0;
    }

    public async Task<int> GetDiscoveryRatingAsync(int discoveryId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT CalculateDiscoveryRating(@DiscoveryId)";
        return await connection.ExecuteScalarAsync<int>(sql, new { DiscoveryId = discoveryId });
    }
}