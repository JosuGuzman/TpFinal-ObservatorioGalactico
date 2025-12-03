namespace Observatorio.Infrastructure.Repositories.Dapper;

public class DiscoveryRepository : BaseRepository, IDiscoveryRepository
{
    public DiscoveryRepository(DapperContext context) : base(context)
    {
    }

    public async Task<Discovery> GetByIdAsync(int id)
    {
        return await WithConnection(async conn =>
        {
            var sql = "CALL sp_get_discovery_by_id(@id)";
            return await conn.QueryFirstOrDefaultAsync<Discovery>(sql, new { id });
        });
    }

    public async Task<IEnumerable<Discovery>> GetAllAsync()
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT d.*, u.* FROM Discoveries d
                LEFT JOIN Users u ON d.ReporterUserID = u.UserID
                ORDER BY d.CreatedAt DESC";

            var result = await conn.QueryAsync<Discovery, User, Discovery>(
                sql,
                (discovery, user) =>
                {
                    discovery.Reporter = user;
                    return discovery;
                },
                splitOn: "UserID"
            );

            return result;
        });
    }

    public async Task<Discovery> AddAsync(Discovery entity)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                CALL sp_create_discovery(
                    @ReporterUserID, @ObjectType, @SuggestedName, @RA, @Dec, @Description
                );
                SELECT LAST_INSERT_ID();";

            var parameters = new
            {
                entity.ReporterUserID,
                ObjectType = entity.ObjectType.ToString(),
                entity.SuggestedName,
                entity.RA,
                entity.Dec,
                entity.Description
            };

            var id = await conn.ExecuteScalarAsync<int>(sql, parameters);
            entity.DiscoveryID = id;
            return entity;
        });
    }

    public async Task UpdateAsync(Discovery entity)
    {
        await WithConnection(async conn =>
        {
            var sql = @"
                UPDATE Discoveries 
                SET ObjectType = @ObjectType,
                    SuggestedName = @SuggestedName,
                    RA = @RA,
                    Dec = @Dec,
                    Description = @Description,
                    Attachments = @Attachments,
                    State = @State,
                    UpdatedAt = @UpdatedAt
                WHERE DiscoveryID = @DiscoveryID";

            await conn.ExecuteAsync(sql, entity);
        });
    }

    public async Task DeleteAsync(int id)
    {
        await WithConnection(async conn =>
        {
            var sql = "CALL sp_delete_discovery(@id)";
            await conn.ExecuteAsync(sql, new { id });
        });
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT COUNT(1) FROM Discoveries WHERE DiscoveryID = @id";
            var count = await conn.ExecuteScalarAsync<int>(sql, new { id });
            return count > 0;
        });
    }

    public async Task<int> CountAsync()
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT COUNT(*) FROM Discoveries";
            return await conn.ExecuteScalarAsync<int>(sql);
        });
    }

    public async Task<IEnumerable<Discovery>> GetByReporterAsync(int userId)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT d.*, u.* FROM Discoveries d
                LEFT JOIN Users u ON d.ReporterUserID = u.UserID
                WHERE d.ReporterUserID = @userId
                ORDER BY d.CreatedAt DESC";

            var result = await conn.QueryAsync<Discovery, User, Discovery>(
                sql,
                (discovery, user) =>
                {
                    discovery.Reporter = user;
                    return discovery;
                },
                new { userId },
                splitOn: "UserID"
            );

            return result;
        });
    }

    public async Task<IEnumerable<Discovery>> GetByStateAsync(string state)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT d.*, u.* FROM Discoveries d
                LEFT JOIN Users u ON d.ReporterUserID = u.UserID
                WHERE d.State = @state
                ORDER BY d.CreatedAt DESC";

            var result = await conn.QueryAsync<Discovery, User, Discovery>(
                sql,
                (discovery, user) =>
                {
                    discovery.Reporter = user;
                    return discovery;
                },
                new { state },
                splitOn: "UserID"
            );

            return result;
        });
    }

    public async Task<IEnumerable<Discovery>> GetPendingAsync()
    {
        return await GetByStateAsync("Pendiente");
    }

    public async Task UpdateStateAsync(int discoveryId, string state)
    {
        await WithConnection(async conn =>
        {
            var sql = "CALL sp_update_discovery_status(@discoveryId, @state)";
            await conn.ExecuteAsync(sql, new { discoveryId, state });
        });
    }

    public async Task<int> CountByStateAsync(string state)
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT COUNT(*) FROM Discoveries WHERE State = @state";
            return await conn.ExecuteScalarAsync<int>(sql, new { state });
        });
    }

    public async Task<IEnumerable<Discovery>> GetTopRatedAsync(int limit)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT d.*, u.* FROM Discoveries d
                LEFT JOIN Users u ON d.ReporterUserID = u.UserID
                LEFT JOIN DiscoveryVotes v ON d.DiscoveryID = v.DiscoveryID
                GROUP BY d.DiscoveryID
                ORDER BY SUM(CASE WHEN v.Vote = 1 THEN 1 ELSE 0 END) DESC
                LIMIT @limit";

            var result = await conn.QueryAsync<Discovery, User, Discovery>(
                sql,
                (discovery, user) =>
                {
                    discovery.Reporter = user;
                    return discovery;
                },
                new { limit },
                splitOn: "UserID"
            );

            return result;
        });
    }
}