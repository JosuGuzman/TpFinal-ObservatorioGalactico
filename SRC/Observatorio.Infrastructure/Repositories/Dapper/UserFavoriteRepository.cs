namespace Observatorio.Infrastructure.Repositories.Dapper;

public class UserFavoriteRepository : BaseRepository, IUserFavoriteRepository
{
    public UserFavoriteRepository(DapperContext context) : base(context)
    {
    }

    public async Task<UserFavorite> GetByIdAsync(int id)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT f.*, u.* FROM UserFavorites f
                LEFT JOIN Users u ON f.UserID = u.UserID
                WHERE f.FavoriteID = @id";

            var result = await conn.QueryAsync<UserFavorite, User, UserFavorite>(
                sql,
                (favorite, user) =>
                {
                    favorite.User = user;
                    return favorite;
                },
                new { id },
                splitOn: "UserID"
            );

            return result.FirstOrDefault();
        });
    }

    public async Task<IEnumerable<UserFavorite>> GetAllAsync()
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT f.*, u.* FROM UserFavorites f
                LEFT JOIN Users u ON f.UserID = u.UserID
                ORDER BY f.CreatedAt DESC";

            var result = await conn.QueryAsync<UserFavorite, User, UserFavorite>(
                sql,
                (favorite, user) =>
                {
                    favorite.User = user;
                    return favorite;
                },
                splitOn: "UserID"
            );

            return result;
        });
    }

    public async Task<UserFavorite> AddAsync(UserFavorite entity)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                CALL sp_add_favorite(
                    @UserID, @ObjectType, @ObjectID
                );
                SELECT LAST_INSERT_ID();";

            var parameters = new
            {
                entity.UserID,
                ObjectType = entity.ObjectType.ToString(),
                entity.ObjectID
            };

            var id = await conn.ExecuteScalarAsync<int>(sql, parameters);
            entity.FavoriteID = id;
            return entity;
        });
    }

    public async Task UpdateAsync(UserFavorite entity)
    {
        await WithConnection(async conn =>
        {
            var sql = @"
                UPDATE UserFavorites 
                SET UserID = @UserID,
                    ObjectType = @ObjectType,
                    ObjectID = @ObjectID
                WHERE FavoriteID = @FavoriteID";

            await conn.ExecuteAsync(sql, entity);
        });
    }

    public async Task DeleteAsync(int id)
    {
        await WithConnection(async conn =>
        {
            var sql = "CALL sp_delete_favorite(@id)";
            await conn.ExecuteAsync(sql, new { id });
        });
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT COUNT(1) FROM UserFavorites WHERE FavoriteID = @id";
            var count = await conn.ExecuteScalarAsync<int>(sql, new { id });
            return count > 0;
        });
    }

    public async Task<int> CountAsync()
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT COUNT(*) FROM UserFavorites";
            return await conn.ExecuteScalarAsync<int>(sql);
        });
    }

    public async Task<IEnumerable<UserFavorite>> GetByUserAsync(int userId)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT f.*, u.* FROM UserFavorites f
                LEFT JOIN Users u ON f.UserID = u.UserID
                WHERE f.UserID = @userId
                ORDER BY f.CreatedAt DESC";

            var result = await conn.QueryAsync<UserFavorite, User, UserFavorite>(
                sql,
                (favorite, user) =>
                {
                    favorite.User = user;
                    return favorite;
                },
                new { userId },
                splitOn: "UserID"
            );

            return result;
        });
    }

    public async Task<UserFavorite> GetByUserAndObjectAsync(int userId, string objectType, int objectId)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT f.*, u.* FROM UserFavorites f
                LEFT JOIN Users u ON f.UserID = u.UserID
                WHERE f.UserID = @userId 
                  AND f.ObjectType = @objectType 
                  AND f.ObjectID = @objectId";

            var result = await conn.QueryAsync<UserFavorite, User, UserFavorite>(
                sql,
                (favorite, user) =>
                {
                    favorite.User = user;
                    return favorite;
                },
                new { userId, objectType, objectId },
                splitOn: "UserID"
            );

            return result.FirstOrDefault();
        });
    }

    public async Task<bool> IsFavoritedAsync(int userId, string objectType, int objectId)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT COUNT(1) FROM UserFavorites 
                WHERE UserID = @userId 
                  AND ObjectType = @objectType 
                  AND ObjectID = @objectId";

            var count = await conn.ExecuteScalarAsync<int>(sql, new { userId, objectType, objectId });
            return count > 0;
        });
    }

    public async Task<int> CountByObjectAsync(string objectType, int objectId)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT COUNT(*) FROM UserFavorites 
                WHERE ObjectType = @objectType 
                  AND ObjectID = @objectId";

            return await conn.ExecuteScalarAsync<int>(sql, new { objectType, objectId });
        });
    }
}