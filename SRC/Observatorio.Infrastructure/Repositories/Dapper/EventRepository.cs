namespace Observatorio.Infrastructure.Repositories.Dapper;

public class EventRepository : BaseRepository, IEventRepository
{
    public EventRepository(DapperContext context) : base(context)
    {
    }

    public async Task<Event> GetByIdAsync(int id)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT e.*, u.* FROM Events e
                LEFT JOIN Users u ON e.CreatedByUserID = u.UserID
                WHERE e.EventID = @id";

            var result = await conn.QueryAsync<Event, User, Event>(
                sql,
                (evt, user) =>
                {
                    evt.CreatedBy = user;
                    return evt;
                },
                new { id },
                splitOn: "UserID"
            );

            return result.FirstOrDefault();
        });
    }

    public async Task<IEnumerable<Event>> GetAllAsync()
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT e.*, u.* FROM Events e
                LEFT JOIN Users u ON e.CreatedByUserID = u.UserID
                ORDER BY e.EventDate DESC";

            var result = await conn.QueryAsync<Event, User, Event>(
                sql,
                (evt, user) =>
                {
                    evt.CreatedBy = user;
                    return evt;
                },
                splitOn: "UserID"
            );

            return result;
        });
    }

    public async Task<Event> AddAsync(Event entity)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                CALL sp_create_event(
                    @Name, @Type, @EventDate, @Description, @CreatedByUserID
                );
                SELECT LAST_INSERT_ID();";

            var parameters = new
            {
                entity.Name,
                Type = entity.Type.ToString(),
                entity.EventDate,
                entity.Description,
                entity.CreatedByUserID
            };

            var id = await conn.ExecuteScalarAsync<int>(sql, parameters);
            entity.EventID = id;
            return entity;
        });
    }

    public async Task UpdateAsync(Event entity)
    {
        await WithConnection(async conn =>
        {
            var sql = @"
                CALL sp_update_event(
                    @EventID, @Name, @Type, @EventDate, @Description
                )";

            var parameters = new
            {
                entity.EventID,
                entity.Name,
                Type = entity.Type.ToString(),
                entity.EventDate,
                entity.Description
            };

            await conn.ExecuteAsync(sql, parameters);
        });
    }

    public async Task DeleteAsync(int id)
    {
        await WithConnection(async conn =>
        {
            var sql = "CALL sp_delete_event(@id)";
            await conn.ExecuteAsync(sql, new { id });
        });
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT COUNT(1) FROM Events WHERE EventID = @id";
            var count = await conn.ExecuteScalarAsync<int>(sql, new { id });
            return count > 0;
        });
    }

    public async Task<int> CountAsync()
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT COUNT(*) FROM Events";
            return await conn.ExecuteScalarAsync<int>(sql);
        });
    }

    public async Task<IEnumerable<Event>> GetUpcomingAsync(DateTime fromDate)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT e.*, u.* FROM Events e
                LEFT JOIN Users u ON e.CreatedByUserID = u.UserID
                WHERE e.EventDate >= @fromDate
                ORDER BY e.EventDate ASC";

            var result = await conn.QueryAsync<Event, User, Event>(
                sql,
                (evt, user) =>
                {
                    evt.CreatedBy = user;
                    return evt;
                },
                new { fromDate },
                splitOn: "UserID"
            );

            return result;
        });
    }

    public async Task<IEnumerable<Event>> GetByTypeAsync(string type)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT e.*, u.* FROM Events e
                LEFT JOIN Users u ON e.CreatedByUserID = u.UserID
                WHERE e.Type = @type
                ORDER BY e.EventDate DESC";

            var result = await conn.QueryAsync<Event, User, Event>(
                sql,
                (evt, user) =>
                {
                    evt.CreatedBy = user;
                    return evt;
                },
                new { type },
                splitOn: "UserID"
            );

            return result;
        });
    }

    public async Task<IEnumerable<Event>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT e.*, u.* FROM Events e
                LEFT JOIN Users u ON e.CreatedByUserID = u.UserID
                WHERE e.EventDate BETWEEN @startDate AND @endDate
                ORDER BY e.EventDate ASC";

            var result = await conn.QueryAsync<Event, User, Event>(
                sql,
                (evt, user) =>
                {
                    evt.CreatedBy = user;
                    return evt;
                },
                new { startDate, endDate },
                splitOn: "UserID"
            );

            return result;
        });
    }

    public async Task<IEnumerable<Event>> GetRecentAsync(int limit)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT e.*, u.* FROM Events e
                LEFT JOIN Users u ON e.CreatedByUserID = u.UserID
                ORDER BY e.CreatedAt DESC
                LIMIT @limit";

            var result = await conn.QueryAsync<Event, User, Event>(
                sql,
                (evt, user) =>
                {
                    evt.CreatedBy = user;
                    return evt;
                },
                new { limit },
                splitOn: "UserID"
            );

            return result;
        });
    }

    public async Task<int> CountUpcomingAsync()
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT COUNT(*) FROM Events WHERE EventDate >= NOW()";
            return await conn.ExecuteScalarAsync<int>(sql);
        });
    }
}