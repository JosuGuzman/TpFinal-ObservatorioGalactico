namespace Observatorio.Infrastructure.Repositories.Dapper;

public class UserRepository : BaseRepository, IUserRepository
{
    public UserRepository(DapperContext context) : base(context)
    {
    }

    public async Task<User> GetByIdAsync(int id)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT u.*, r.* FROM Users u
                LEFT JOIN Roles r ON u.RoleID = r.RoleID
                WHERE u.UserID = @id";

            var result = await conn.QueryAsync<User, Role, User>(
                sql,
                (user, role) =>
                {
                    user.Role = role;
                    return user;
                },
                new { id },
                splitOn: "RoleID"
            );

            return result.FirstOrDefault();
        });
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT u.*, r.* FROM Users u
                LEFT JOIN Roles r ON u.RoleID = r.RoleID
                ORDER BY u.CreatedAt DESC";

            var result = await conn.QueryAsync<User, Role, User>(
                sql,
                (user, role) =>
                {
                    user.Role = role;
                    return user;
                },
                splitOn: "RoleID"
            );

            return result;
        });
    }

    public async Task<User> AddAsync(User entity)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                INSERT INTO Users (Email, UserName, PasswordHash, IsActive, RoleID, ApiKey, CreatedAt, LastLogin)
                VALUES (@Email, @UserName, @PasswordHash, @IsActive, @RoleID, @ApiKey, @CreatedAt, @LastLogin);
                SELECT LAST_INSERT_ID();";

            var id = await conn.ExecuteScalarAsync<int>(sql, entity);
            entity.UserID = id;
            return entity;
        });
    }

    public async Task UpdateAsync(User entity)
    {
        await WithConnection(async conn =>
        {
            var sql = @"
                UPDATE Users 
                SET Email = @Email, 
                    UserName = @UserName, 
                    PasswordHash = @PasswordHash, 
                    IsActive = @IsActive, 
                    RoleID = @RoleID, 
                    ApiKey = @ApiKey, 
                    LastLogin = @LastLogin
                WHERE UserID = @UserID";

            await conn.ExecuteAsync(sql, entity);
        });
    }

    public async Task DeleteAsync(int id)
    {
        await WithConnection(async conn =>
        {
            var sql = "DELETE FROM Users WHERE UserID = @id";
            await conn.ExecuteAsync(sql, new { id });
        });
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT COUNT(1) FROM Users WHERE UserID = @id";
            var count = await conn.ExecuteScalarAsync<int>(sql, new { id });
            return count > 0;
        });
    }

    public async Task<int> CountAsync()
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT COUNT(*) FROM Users";
            return await conn.ExecuteScalarAsync<int>(sql);
        });
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT u.*, r.* FROM Users u
                LEFT JOIN Roles r ON u.RoleID = r.RoleID
                WHERE u.Email = @email";

            var result = await conn.QueryAsync<User, Role, User>(
                sql,
                (user, role) =>
                {
                    user.Role = role;
                    return user;
                },
                new { email },
                splitOn: "RoleID"
            );

            return result.FirstOrDefault();
        });
    }

    public async Task<User> GetByUsernameAsync(string username)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT u.*, r.* FROM Users u
                LEFT JOIN Roles r ON u.RoleID = r.RoleID
                WHERE u.UserName = @username";

            var result = await conn.QueryAsync<User, Role, User>(
                sql,
                (user, role) =>
                {
                    user.Role = role;
                    return user;
                },
                new { username },
                splitOn: "RoleID"
            );

            return result.FirstOrDefault();
        });
    }

    public async Task<User> GetByApiKeyAsync(string apiKey)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT u.*, r.* FROM Users u
                LEFT JOIN Roles r ON u.RoleID = r.RoleID
                WHERE u.ApiKey = @apiKey";

            var result = await conn.QueryAsync<User, Role, User>(
                sql,
                (user, role) =>
                {
                    user.Role = role;
                    return user;
                },
                new { apiKey },
                splitOn: "RoleID"
            );

            return result.FirstOrDefault();
        });
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT COUNT(1) FROM Users WHERE Email = @email";
            var count = await conn.ExecuteScalarAsync<int>(sql, new { email });
            return count > 0;
        });
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await WithConnection(async conn =>
        {
            var sql = "SELECT COUNT(1) FROM Users WHERE UserName = @username";
            var count = await conn.ExecuteScalarAsync<int>(sql, new { username });
            return count > 0;
        });
    }

    public async Task UpdateLastLoginAsync(int userId)
    {
        await WithConnection(async conn =>
        {
            var sql = "UPDATE Users SET LastLogin = NOW() WHERE UserID = @userId";
            await conn.ExecuteAsync(sql, new { userId });
        });
    }

    public async Task<IEnumerable<User>> GetByRoleAsync(int roleId)
    {
        return await WithConnection(async conn =>
        {
            var sql = @"
                SELECT u.*, r.* FROM Users u
                LEFT JOIN Roles r ON u.RoleID = r.RoleID
                WHERE u.RoleID = @roleId
                ORDER BY u.UserName";

            var result = await conn.QueryAsync<User, Role, User>(
                sql,
                (user, role) =>
                {
                    user.Role = role;
                    return user;
                },
                new { roleId },
                splitOn: "RoleID"
            );

            return result;
        });
    }

    public async Task DeactivateUserAsync(int userId)
    {
        await WithConnection(async conn =>
        {
            var sql = "UPDATE Users SET IsActive = 0 WHERE UserID = @userId";
            await conn.ExecuteAsync(sql, new { userId });
        });
    }

    public async Task ActivateUserAsync(int userId)
    {
        await WithConnection(async conn =>
        {
            var sql = "UPDATE Users SET IsActive = 1 WHERE UserID = @userId";
            await conn.ExecuteAsync(sql, new { userId });
        });
    }
}