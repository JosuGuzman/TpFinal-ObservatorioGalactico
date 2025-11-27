namespace WatchTower.Infrastructure.Data.Repositories;

public class CelestialBodyRepository : ICelestialBodyRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CelestialBodyRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<CelestialBody?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM CelestialBodies WHERE BodyId = @Id";
        return await connection.QueryFirstOrDefaultAsync<CelestialBody>(sql, new { Id = id });
    }

    public async Task<IEnumerable<CelestialBody>> SearchAsync(CelestialBodySearchRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var parameters = new
        {
            search_term = request.SearchTerm,
            body_type = request.BodyType,
            max_distance = request.MaxDistance,
            min_magnitude = request.MinMagnitude
        };

        return await connection.QueryAsync<CelestialBody>(
            "CALL SearchCelestialBodies(@search_term, @body_type, @max_distance, @min_magnitude)",
            parameters);
    }

    public async Task<int> CreateAsync(CelestialBody body)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO CelestialBodies 
            (Name, Type, SubType, Constellation, RightAscension, Declination, Distance, 
             ApparentMagnitude, AbsoluteMagnitude, Mass, Radius, Temperature, Description, 
             DiscoveryDate, NASA_ImageURL, IsVerified, CreatedBy) 
            VALUES 
            (@Name, @Type, @SubType, @Constellation, @RightAscension, @Declination, @Distance,
             @ApparentMagnitude, @AbsoluteMagnitude, @Mass, @Radius, @Temperature, @Description,
             @DiscoveryDate, @NASA_ImageURL, @IsVerified, @CreatedBy);
            SELECT LAST_INSERT_ID();";
        
        return await connection.ExecuteScalarAsync<int>(sql, body);
    }

    public async Task UpdateAsync(CelestialBody body)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE CelestialBodies SET 
            Name = @Name, Type = @Type, SubType = @SubType, Constellation = @Constellation,
            RightAscension = @RightAscension, Declination = @Declination, Distance = @Distance,
            ApparentMagnitude = @ApparentMagnitude, AbsoluteMagnitude = @AbsoluteMagnitude,
            Mass = @Mass, Radius = @Radius, Temperature = @Temperature, Description = @Description,
            DiscoveryDate = @DiscoveryDate, NASA_ImageURL = @NASA_ImageURL, IsVerified = @IsVerified
            WHERE BodyId = @BodyId";
        
        await connection.ExecuteAsync(sql, body);
    }

    public async Task<bool> VerifyBodyAsync(int bodyId, int verifiedBy)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "UPDATE CelestialBodies SET IsVerified = 1 WHERE BodyId = @BodyId";
        var affected = await connection.ExecuteAsync(sql, new { BodyId = bodyId });
        return affected > 0;
    }

    public async Task<IEnumerable<CelestialBody>> GetRecentBodiesAsync(int count)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM CelestialBodies WHERE IsVerified = 1 ORDER BY CreatedAt DESC LIMIT @Count";
        return await connection.QueryAsync<CelestialBody>(sql, new { Count = count });
    }

    public async Task<int> GetTotalCountAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT COUNT(*) FROM CelestialBodies WHERE IsVerified = 1";
        return await connection.ExecuteScalarAsync<int>(sql);
    }

    public async Task<IEnumerable<CelestialBody>> GetBodiesByTypeAsync(CelestialBodyType type)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM CelestialBodies WHERE Type = @Type AND IsVerified = 1";
        return await connection.QueryAsync<CelestialBody>(sql, new { Type = type.ToString() });
    }
}