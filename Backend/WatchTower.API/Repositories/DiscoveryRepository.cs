using Dapper;
using WatchTower.API.Data;
using WatchTower.API.Models.Entities;
using WatchTower.API.Models.DTOs;

namespace WatchTower.API.Repositories;

public class DiscoveryRepository : IDiscoveryRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DiscoveryRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<DiscoveryResponse>> GetDiscoveriesAsync(string? searchTerm = null, string? status = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var parameters = new
        {
            search_term = searchTerm,
            status_filter = status,
            min_rating = (int?)null
        };

        return await connection.QueryAsync<DiscoveryResponse>(
            "CALL SearchDiscoveries(@search_term, @status_filter, @min_rating)",
            parameters);
    }

    public async Task<Discovery?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Discovery>(
            "SELECT * FROM Discoveries WHERE DiscoveryId = @Id", new { Id = id });
    }

    public async Task<int> AddDiscoveryAsync(Discovery discovery)
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

    public async Task<bool> AddVoteAsync(int discoveryId, int userId, string voteType)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        try
        {
            var sql = @"INSERT INTO Votes (DiscoveryId, UserId, VoteType) 
                        VALUES (@DiscoveryId, @UserId, @VoteType)
                        ON DUPLICATE KEY UPDATE VoteType = @VoteType";
            
            var affected = await connection.ExecuteAsync(sql, new 
            { 
                DiscoveryId = discoveryId, 
                UserId = userId, 
                VoteType = voteType 
            });
            
            return affected > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task UpdateDiscoveryAsync(Discovery discovery)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"UPDATE Discoveries SET 
                    Title = @Title, Description = @Description, Status = @Status,
                    VerifiedAt = @VerifiedAt, VerifiedBy = @VerifiedBy
                    WHERE DiscoveryId = @DiscoveryId";
        
        await connection.ExecuteAsync(sql, discovery);
    }
}