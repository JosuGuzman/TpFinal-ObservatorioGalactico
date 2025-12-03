namespace Observatorio.Infrastructure.Repositories.Dapper;

public class SystemLogRepository : BaseRepository, ISystemLogRepository
{
    public SystemLogRepository(DapperContext context) : base(context)
    {
    }

    public async Task<SystemLog> GetByIdAsync(int id)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT l.*, u.* FROM SystemLogs l
                LEFT JOIN Users u ON l.UserID = u.UserID
                WHERE l.LogID = @id";

            var result = await conn.QueryAsync<SystemLog, User, SystemLog>(
                sql,
                (log, user) =>
                {
                    log.User = user;
                    return log;
                },
                new { id },
                splitOn: "UserID"
            );

            return result.FirstOrDefault();
        });
    }

    public async Task<IEnumerable<SystemLog>> GetAllAsync()
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT l.*, u.* FROM SystemLogs l
                LEFT JOIN Users u ON l.UserID = u.UserID
                ORDER BY l.Timestamp DESC";

            var result = await conn.QueryAsync<SystemLog, User, SystemLog>(
                sql,
                (log, user) =>
                {
                    log.User = user;
                    return log;
                },
                splitOn: "UserID"
            );

            return result;
        });
    }

    public async Task<SystemLog> AddAsync(SystemLog entity)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                CALL sp_add_log(
                    @UserID, @EventType, @Description, @IPAddress, @Status
                );
                SELECT LAST_INSERT_ID();";

            var parameters = new
            {
                entity.UserID,
                entity.EventType,
                entity.Description,
                entity.IPAddress,
                entity.Status
            };

            var id = await conn.ExecuteScalarAsync<long>(sql, parameters);
            entity.LogID = id;
            return entity;
        });
    }

    public async Task UpdateAsync(SystemLog entity)
    {
        await WithConnection(async conn =>
        {
            var sql = @"
                UPDATE SystemLogs 
                SET UserID = @UserID,
                    EventType = @EventType,
                    Description = @Description,
                    IPAddress = @IPAddress,
                    Status = @Status
                WHERE LogID = @LogID";

            await conn.ExecuteAsync(sql, entity);
        });
    }

    public async Task DeleteAsync(int id)
    {
        await WithConnection(async conn =>
        {
            var sql = "DELETE FROM SystemLogs WHERE LogID = @id";
            await conn.ExecuteAsync(sql, new { id });
        });
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT COUNT(1) FROM SystemLogs WHERE LogID = @id";
            var count = await conn.ExecuteScalarAsync<int>(sql, new { id });
            return count > 0;
        });
    }

    public async Task<int> CountAsync()
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT COUNT(*) FROM SystemLogs";
            return await conn.ExecuteScalarAsync<int>(sql);
        });
    }

    public async Task<IEnumerable<SystemLog>> GetByUserAsync(int userId)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT l.*, u.* FROM SystemLogs l
                LEFT JOIN Users u ON l.UserID = u.UserID
                WHERE l.UserID = @userId
                ORDER BY l.Timestamp DESC";

            var result = await conn.QueryAsync<SystemLog, User, SystemLog>(
                sql,
                (log, user) =>
                {
                    log.User = user;
                    return log;
                },
                new { userId },
                splitOn: "UserID"
            );

            return result;
        });
    }

    public async Task<IEnumerable<SystemLog>> GetByEventTypeAsync(string eventType)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT l.*, u.* FROM SystemLogs l
                LEFT JOIN Users u ON l.UserID = u.UserID
                WHERE l.EventType = @eventType
                ORDER BY l.Timestamp DESC";

            var result = await conn.QueryAsync<SystemLog, User, SystemLog>(
                sql,
                (log, user) =>
                {
                    log.User = user;
                    return log;
                },
                new { eventType },
                splitOn: "UserID"
            );

            return result;
        });
    }

    public async Task<IEnumerable<SystemLog>> GetRecentAsync(int limit)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT l.*, u.* FROM SystemLogs l
                LEFT JOIN Users u ON l.UserID = u.UserID
                ORDER BY l.Timestamp DESC
                LIMIT @limit";

            var result = await conn.QueryAsync<SystemLog, User, SystemLog>(
                sql,
                (log, user) =>
                {
                    log.User = user;
                    return log;
                },
                new { limit },
                splitOn: "UserID"
            );

            return result;
        });
    }

    public async Task ClearOldLogsAsync(DateTime olderThan)
    {
        await WithConnection(async conn =>
        {
            var sql = "DELETE FROM SystemLogs WHERE Timestamp < @olderThan";
            await conn.ExecuteAsync(sql, new { olderThan });
        });
    }
}