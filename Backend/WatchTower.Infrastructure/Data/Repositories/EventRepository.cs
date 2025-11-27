namespace WatchTower.Infrastructure.Data.Repositories;

public class EventRepository : IEventRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public EventRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Event?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT e.*, u.Username as CreatorName
            FROM Events e
            INNER JOIN Users u ON e.CreatedBy = u.UserId
            WHERE e.EventId = @Id";
        
        return await connection.QueryFirstOrDefaultAsync<Event>(sql, new { Id = id });
    }

    public async Task<IEnumerable<Event>> GetUpcomingEventsAsync(int count)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            SELECT e.*, u.Username as CreatorName
            FROM Events e
            INNER JOIN Users u ON e.CreatedBy = u.UserId
            WHERE e.StartDate >= NOW() AND e.IsActive = 1
            ORDER BY e.StartDate ASC
            LIMIT @Count";
        
        return await connection.QueryAsync<Event>(sql, new { Count = count });
    }

    public async Task<IEnumerable<Event>> SearchEventsAsync(DateTime? startDate, DateTime? endDate, EventType? eventType)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"
            SELECT e.*, u.Username as CreatorName
            FROM Events e
            INNER JOIN Users u ON e.CreatedBy = u.UserId
            WHERE 1=1";

        var parameters = new DynamicParameters();

        if (startDate.HasValue)
        {
            sql += " AND e.StartDate >= @StartDate";
            parameters.Add("StartDate", startDate.Value);
        }

        if (endDate.HasValue)
        {
            sql += " AND e.StartDate <= @EndDate";
            parameters.Add("EndDate", endDate.Value);
        }

        if (eventType.HasValue)
        {
            sql += " AND e.EventType = @EventType";
            parameters.Add("EventType", eventType.Value.ToString());
        }

        sql += " ORDER BY e.StartDate ASC";

        return await connection.QueryAsync<Event>(sql, parameters);
    }

    public async Task<int> CreateAsync(Event eventEntity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO Events (Title, Description, EventType, StartDate, EndDate, Location, Visibility, CreatedBy)
            VALUES (@Title, @Description, @EventType, @StartDate, @EndDate, @Location, @Visibility, @CreatedBy);
            SELECT LAST_INSERT_ID();";
        
        return await connection.ExecuteScalarAsync<int>(sql, eventEntity);
    }

    public async Task UpdateAsync(Event eventEntity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE Events SET 
            Title = @Title, Description = @Description, EventType = @EventType,
            StartDate = @StartDate, EndDate = @EndDate, Location = @Location,
            Visibility = @Visibility, IsActive = @IsActive
            WHERE EventId = @EventId";
        
        await connection.ExecuteAsync(sql, eventEntity);
    }

    public async Task<bool> DeactivateAsync(int eventId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "UPDATE Events SET IsActive = 0 WHERE EventId = @EventId";
        var affected = await connection.ExecuteAsync(sql, new { EventId = eventId });
        return affected > 0;
    }
}