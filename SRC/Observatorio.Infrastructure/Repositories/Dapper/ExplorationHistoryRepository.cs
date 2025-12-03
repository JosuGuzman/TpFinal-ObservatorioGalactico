namespace Observatorio.Infrastructure.Repositories.Dapper;

public class ExplorationHistoryRepository : BaseRepository, IExplorationHistoryRepository
{
    public ExplorationHistoryRepository(DapperContext context) : base(context)
    {
    }

    public async Task<ExplorationHistory> GetByIdAsync(int id)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT h.*, u.* FROM ExplorationHistory h
                LEFT JOIN Users u ON h.UserID = u.UserID
                WHERE h.HistoryID = @id";

            var result = await conn.QueryAsync<ExplorationHistory, User, ExplorationHistory>(
                sql,
                (history, user) =>
                {
                    history.User = user;
                    return history;
                },
                new { id },
                splitOn: "UserID"
            );

            return result.FirstOrDefault();
        });
    }

    public async Task<IEnumerable<ExplorationHistory>> GetAllAsync()
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT h.*, u.* FROM ExplorationHistory h
                LEFT JOIN Users u ON h.UserID = u.UserID
                ORDER BY h.AccessedAt DESC";

            var result = await conn.QueryAsync<ExplorationHistory, User, ExplorationHistory>(
                sql,
                (history, user) =>
                {
                    history.User = user;
                    return history;
                },
                splitOn: "UserID"
            );

            return result;
        });
    }

    public async Task<ExplorationHistory> AddAsync(ExplorationHistory entity)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                CALL sp_add_history(
                    @UserID, @ObjectType, @ObjectID, @DurationSeconds, @SearchCriteria
                );
                SELECT LAST_INSERT_ID();";

            var parameters = new
            {
                entity.UserID,
                ObjectType = entity.ObjectType.ToString(),
                entity.ObjectID,
                entity.DurationSeconds,
                entity.SearchCriteria
            };

            var id = await conn.ExecuteScalarAsync<long>(sql, parameters);
            entity.HistoryID = id;
            return entity;
        });
    }

    public async Task UpdateAsync(ExplorationHistory entity)
    {
        await WithConnection(async conn =>
        {
            var sql = @"
                UPDATE ExplorationHistory 
                SET DurationSeconds = @DurationSeconds,
                    SearchCriteria = @SearchCriteria
                WHERE HistoryID = @HistoryID";

            await conn.ExecuteAsync(sql, entity);
        });
    }

    public async Task DeleteAsync(int id)
    {
        await WithConnection(async conn =>
        {
            var sql = "DELETE FROM ExplorationHistory WHERE HistoryID = @id";
            await conn.ExecuteAsync(sql, new { id });
        });
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT COUNT(1) FROM ExplorationHistory WHERE HistoryID = @id";
            var count = await conn.ExecuteScalarAsync<int>(sql, new { id });
            return count > 0;
        });
    }

    public async Task<int> CountAsync()
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT COUNT(*) FROM ExplorationHistory";
            return await conn.ExecuteScalarAsync<int>(sql);
        });
    }

    public async Task<IEnumerable<ExplorationHistory>> GetByUserAsync(int userId, int limit = 50)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT h.*, u.* FROM ExplorationHistory h
                LEFT JOIN Users u ON h.UserID = u.UserID
                WHERE h.UserID = @userId
                ORDER BY h.AccessedAt DESC
                LIMIT @limit";

            var result = await conn.QueryAsync<ExplorationHistory, User, ExplorationHistory>(
                sql,
                (history, user) =>
                {
                    history.User = user;
                    return history;
                },
                new { userId, limit },
                splitOn: "UserID"
            );

            return result;
        });
    }

    public async Task<IEnumerable<ExplorationHistory>> GetRecentByUserAsync(int userId, DateTime since)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT h.*, u.* FROM ExplorationHistory h
                LEFT JOIN Users u ON h.UserID = u.UserID
                WHERE h.UserID = @userId AND h.AccessedAt >= @since
                ORDER BY h.AccessedAt DESC";

            var result = await conn.QueryAsync<ExplorationHistory, User, ExplorationHistory>(
                sql,
                (history, user) =>
                {
                    history.User = user;
                    return history;
                },
                new { userId, since },
                splitOn: "UserID"
            );

            return result;
        });
    }

    public async Task<int> ClearUserHistoryAsync(int userId)
    {
        return await WithConnection(async conn =>
        {
            var sql = "DELETE FROM ExplorationHistory WHERE UserID = @userId";
            return await conn.ExecuteAsync(sql, new { userId });
        });
    }

    public async Task<IEnumerable<ExplorationHistory>> GetByObjectAsync(string objectType, int objectId)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT h.*, u.* FROM ExplorationHistory h
                LEFT JOIN Users u ON h.UserID = u.UserID
                WHERE h.ObjectType = @objectType AND h.ObjectID = @objectId
                ORDER BY h.AccessedAt DESC";

            var result = await conn.QueryAsync<ExplorationHistory, User, ExplorationHistory>(
                sql,
                (history, user) =>
                {
                    history.User = user;
                    return history;
                },
                new { objectType, objectId },
                splitOn: "UserID"
            );

            return result;
        });
    }
}