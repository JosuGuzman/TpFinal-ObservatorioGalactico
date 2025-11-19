using Dapper;
using WatchTower.API.Data;
using WatchTower.API.Models.Entities;
using WatchTower.API.Models.DTOs;

namespace WatchTower.API.Repositories;

public class CelestialBodyRepository : ICelestialBodyRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CelestialBodyRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<CelestialBodyResponse>> SearchCelestialBodiesAsync(CelestialBodySearchRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var parameters = new
        {
            search_term = request.SearchTerm,
            body_type = request.BodyType,
            max_distance = request.MaxDistance,
            min_magnitude = request.MinMagnitude
        };

        return await connection.QueryAsync<CelestialBodyResponse>(
            "CALL SearchCelestialBodies(@search_term, @body_type, @max_distance, @min_magnitude)",
            parameters);
    }

    public async Task<CelestialBody?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<CelestialBody>(
            "SELECT * FROM CelestialBodies WHERE BodyId = @Id", new { Id = id });
    }

    public async Task<IEnumerable<CelestialBody>> GetRecentBodiesAsync(int count)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<CelestialBody>(
            "SELECT * FROM CelestialBodies WHERE IsVerified = 1 ORDER BY CreatedAt DESC LIMIT @Count",
            new { Count = count });
    }
}