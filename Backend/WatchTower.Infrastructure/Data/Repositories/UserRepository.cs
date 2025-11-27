namespace WatchTower.Infrastructure.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> GetByIdAsync(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM Users WHERE UserId = @UserId";
        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { UserId = userId });
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM Users WHERE Username = @Username";
        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Username = username });
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM Users WHERE Email = @Email";
        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
    }

    public async Task<int> CreateAsync(User user)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, Role) 
            VALUES (@Username, @Email, @PasswordHash, @FirstName, @LastName, @Role);
            SELECT LAST_INSERT_ID();";
        
        return await connection.ExecuteScalarAsync<int>(sql, user);
    }

    public async Task UpdateAsync(User user)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = @"
            UPDATE Users SET 
            Username = @Username, Email = @Email, FirstName = @FirstName, 
            LastName = @LastName, Role = @Role, IsActive = @IsActive, 
            LastLogin = @LastLogin 
            WHERE UserId = @UserId";
        
        await connection.ExecuteAsync(sql, user);
    }

    public async Task<bool> UserExistsAsync(string username, string email)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT COUNT(1) FROM Users WHERE Username = @Username OR Email = @Email";
        var count = await connection.ExecuteScalarAsync<int>(sql, new { Username = username, Email = email });
        return count > 0;
    }

    public async Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM Users WHERE Role = @Role AND IsActive = 1";
        return await connection.QueryAsync<User>(sql, new { Role = role.ToString() });
    }

    public async Task<bool> DeactivateUserAsync(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "UPDATE Users SET IsActive = 0 WHERE UserId = @UserId";
        var affected = await connection.ExecuteAsync(sql, new { UserId = userId });
        return affected > 0;
    }

    public async Task UpdateLastLoginAsync(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string sql = "UPDATE Users SET LastLogin = NOW() WHERE UserId = @UserId";
        await connection.ExecuteAsync(sql, new { UserId = userId });
    }
}