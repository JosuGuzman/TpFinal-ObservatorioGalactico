namespace Observatorio.Infrastructure.Repositories.Dapper;

public class SavedSearchRepository : BaseRepository, ISavedSearchRepository
{
    public SavedSearchRepository(DapperContext context) : base(context)
    {
    }

    public async Task<SavedSearch> GetByIdAsync(int id)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT s.*, u.* FROM SavedSearches s
                LEFT JOIN Users u ON s.UserID = u.UserID
                WHERE s.SavedSearchID = @id";

            var result = await conn.QueryAsync<SavedSearch, User, SavedSearch>(
                sql,
                (search, user) =>
                {
                    search.User = user;
                    return search;
                },
                new { id },
                splitOn: "UserID"
            );

            return result.FirstOrDefault();
        });
    }

    public async Task<IEnumerable<SavedSearch>> GetAllAsync()
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT s.*, u.* FROM SavedSearches s
                LEFT JOIN Users u ON s.UserID = u.UserID
                ORDER BY s.CreatedAt DESC";

            var result = await conn.QueryAsync<SavedSearch, User, SavedSearch>(
                sql,
                (search, user) =>
                {
                    search.User = user;
                    return search;
                },
                splitOn: "UserID"
            );

            return result;
        });
    }

    public async Task<SavedSearch> AddAsync(SavedSearch entity)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                CALL sp_save_search(
                    @UserID, @Name, @Criteria
                );
                SELECT LAST_INSERT_ID();";

            var parameters = new
            {
                entity.UserID,
                entity.Name,
                entity.Criteria
            };

            var id = await conn.ExecuteScalarAsync<int>(sql, parameters);
            entity.SavedSearchID = id;
            return entity;
        });
    }

    public async Task UpdateAsync(SavedSearch entity)
    {
        await WithConnection(async conn =>
        {
            var sql = @"
                UPDATE SavedSearches 
                SET Name = @Name,
                    Criteria = @Criteria
                WHERE SavedSearchID = @SavedSearchID";

            await conn.ExecuteAsync(sql, entity);
        });
    }

    public async Task DeleteAsync(int id)
    {
        await WithConnection(async conn =>
        {
            var sql = "CALL sp_delete_saved_search(@id)";
            await conn.ExecuteAsync(sql, new { id });
        });
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT COUNT(1) FROM SavedSearches WHERE SavedSearchID = @id";
            var count = await conn.ExecuteScalarAsync<int>(sql, new { id });
            return count > 0;
        });
    }

    public async Task<int> CountAsync()
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT COUNT(*) FROM SavedSearches";
            return await conn.ExecuteScalarAsync<int>(sql);
        });
    }

    public async Task<IEnumerable<SavedSearch>> GetByUserAsync(int userId)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT s.*, u.* FROM SavedSearches s
                LEFT JOIN Users u ON s.UserID = u.UserID
                WHERE s.UserID = @userId
                ORDER BY s.CreatedAt DESC";

            var result = await conn.QueryAsync<SavedSearch, User, SavedSearch>(
                sql,
                (search, user) =>
                {
                    search.User = user;
                    return search;
                },
                new { userId },
                splitOn: "UserID"
            );

            return result;
        });
    }

    public async Task<SavedSearch> GetByNameAsync(int userId, string name)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT s.*, u.* FROM SavedSearches s
                LEFT JOIN Users u ON s.UserID = u.UserID
                WHERE s.UserID = @userId AND s.Name = @name";

            var result = await conn.QueryAsync<SavedSearch, User, SavedSearch>(
                sql,
                (search, user) =>
                {
                    search.User = user;
                    return search;
                },
                new { userId, name },
                splitOn: "UserID"
            );

            return result.FirstOrDefault();
        });
    }

    public async Task<int> DeleteByUserAsync(int userId)
    {
        return await WithConnection(async conn =>
        {
            var sql = "DELETE FROM SavedSearches WHERE UserID = @userId";
            return await conn.ExecuteAsync(sql, new { userId });
        });
    }
}